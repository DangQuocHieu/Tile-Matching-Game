using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEditor;
using UnityEngine;

public class BoardManager : Singleton<BoardManager>, IMessageHandle
{
    [SerializeField] private int _boardWidth;
    [SerializeField] private int _boardHeight;
    [SerializeField] private Diamond[] _diamondPrefabs;
    [SerializeField] private Transform _diamondContainer;
    private List<Diamond> _availableDiamond;
    public int BoardWidth => _boardWidth;
    public int BoardHeight => _boardHeight;
    private Diamond[,] _board;
    public Diamond[,] Board => _board;
    private MatchFinder _matchFinder;
    private HashSet<Diamond> _allMatches = new HashSet<Diamond>();
    private Dictionary<DiamondType, int> _matchDiamondCount = new Dictionary<DiamondType, int>();
    [Header("Diamond Animation Config")]
    [SerializeField] ScaleAmimationSO _scaleDiamondSO;
    [SerializeField] SwapAnimationSO _swapDiamondSO;
    [SerializeField] DropAnimationSO _dropDiamondSO;
    void Start()
    {
        _matchFinder = GetComponent<MatchFinder>();
        GenerateBoard();
    }

    void OnEnable()
    {
        MessageManager.AddSubcriber(GameMessageType.OnDiamondSwapped, this);
        MessageManager.AddSubcriber(GameMessageType.OnTurnStartDelay, this);
    }

    void OnDisable()
    {
        MessageManager.RemoveSubcriber(GameMessageType.OnDiamondSwapped, this);
        MessageManager.RemoveSubcriber(GameMessageType.OnTurnStartDelay, this);
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GenerateBoard();
        }
    }
    private void GenerateBoard()
    {
        ClearBoard();
        _board = new Diamond[_boardHeight, _boardWidth];
        for (int y = 0; y < _boardHeight; y++)
        {
            for (int x = 0; x < _boardWidth; x++)
            {
                _availableDiamond = _diamondPrefabs.ToList();
                if (HasMatchAtRow(x, y))
                {
                    _availableDiamond = _availableDiamond.Where(t => t.DiamondType != _board[y, x - 1].DiamondType).ToList();
                }
                if (HasMatchAtColumn(x, y))
                {
                    _availableDiamond = _availableDiamond.Where(t => t.DiamondType != _board[y - 1, x].DiamondType).ToList();
                }
                Diamond newDiamond = Instantiate(_availableDiamond[UnityEngine.Random.Range(0, _availableDiamond.Count)],
                    new Vector3(x, y, 0), Quaternion.identity, _diamondContainer);
                _board[y, x] = newDiamond;
            }
        }
    }

    private bool HasMatchAtRow(int x, int y)
    {
        if (x >= 2)
        {
            if (_board[y, x - 1].DiamondType == _board[y, x - 2].DiamondType) return true;
        }
        return false;
    }

    private bool HasMatchAtColumn(int x, int y)
    {
        if (y >= 2)
        {
            if (_board[y - 1, x].DiamondType == _board[y - 2, x].DiamondType) return true;
        }
        return false;
    }

    private void ClearBoard()
    {
        if (_board == null) return;
        for (int y = 0; y < _boardHeight; y++)
        {
            for (int x = 0; x < _boardWidth; x++)
            {
                if (_board[y, x] != null)
                {
                    Destroy(_board[y, x].gameObject);
                    _board[y, x] = null;
                }
            }
        }
    }

    public void Handle(Message message)
    {
        switch (message.type)
        {
            case GameMessageType.OnDiamondSwapped:
                GameObject previousDiamond = (GameObject)message.data[0];
                GameObject currentDiamond = (GameObject)message.data[1];
                StartCoroutine(ProcessSwapping(previousDiamond, currentDiamond));
                break;
            case GameMessageType.OnTurnStartDelay:
                _diamondContainer.gameObject.SetActive(true);
                ResetDiamondCount();
                break;
        }
    }

    private IEnumerator ProcessSwapping(GameObject previousDiamond, GameObject currentDiamond)
    {
        yield return _swapDiamondSO.Swap(previousDiamond, currentDiamond, () =>
        {
            SwapDiamond(previousDiamond.transform.position, currentDiamond.transform.position);
        });
        _allMatches = _matchFinder.FindMatches(_boardWidth, _boardHeight, _board);
        if (_allMatches != null && _allMatches.Count > 0)
        {
            while (_allMatches != null && _allMatches.Count > 0)
            {
                yield return ClearAllMatchDiamond();
                yield return CollapseBoard();
                yield return RefillBoard();
                _allMatches = _matchFinder.FindMatches(_boardWidth, _boardHeight, _board);
            }
            //Diamond Completely Match
            yield return ResolveMatches();
            yield return ApplyEffect();

        }
        else
        {
            yield return _swapDiamondSO.Swap(previousDiamond, currentDiamond, () =>
            {
                SwapDiamond(previousDiamond.transform.position, currentDiamond.transform.position);
                MessageManager.SendMessage(new Message(GameMessageType.OnSwappedFail));
            });

        }
    }

    private void SwapDiamond(Vector3 prev, Vector3 curr)
    {
        Diamond temp = _board[(int)prev.y, (int)prev.x];
        _board[(int)prev.y, (int)prev.x] = _board[(int)curr.y, (int)curr.x];
        _board[(int)curr.y, (int)curr.x] = temp;
    }

    // private void DrawBoard()
    // {
    //     string boardString = "";
    //     for (int y = 0; y < _boardHeight; y++)
    //     {
    //         for (int x = 0; x < _boardWidth; x++)
    //         {
    //             if (_board[_boardHeight - y - 1, x] != null)
    //             {
    //                 boardString += (int)_board[_boardHeight - y - 1, x].DiamondType + " ";
    //             }
    //             else
    //             {
    //                 boardString += "0" + " ";
    //             }
    //         }
    //         boardString += "\n";
    //     }
    // }

    private IEnumerator ClearAllMatchDiamond()
    {
        Sequence sequence = DOTween.Sequence();
        foreach (var diamond in _allMatches)
        {
            sequence.Join(_scaleDiamondSO.ScaleOut(diamond.gameObject, () =>
            {
                MessageManager.SendMessage(new Message(GameMessageType.OnDiamondMatched, new object[] { diamond.DiamondType }));
                if (!_matchDiamondCount.ContainsKey(diamond.DiamondType))
                {
                    _matchDiamondCount[diamond.DiamondType] = 1;
                }
                else _matchDiamondCount[diamond.DiamondType]++;
                Destroy(diamond.gameObject);

            }));
        }
        yield return sequence.Play().WaitForCompletion();
        yield return new WaitForEndOfFrame();
    }
    private IEnumerator CollapseBoard()
    {
        Sequence sequence = DOTween.Sequence();
        for (int x = 0; x < _boardWidth; x++)
        {
            for (int y = 0; y < _boardHeight; y++)
            {
                if (_board[y, x] == null)
                {
                    for (int index = y + 1; index < _boardHeight; index++)
                    {
                        if (_board[index, x] != null)
                        {
                            _board[y, x] = _board[index, x];
                            _board[index, x] = null;
                            sequence.Join(_dropDiamondSO.Drop(_board[y, x].gameObject, y));
                            break;
                        }
                    }
                }
            }
        }
        yield return sequence.Play().WaitForCompletion();
        yield return new WaitForEndOfFrame();
    }

    private IEnumerator RefillBoard()
    {
        Sequence sequence = DOTween.Sequence();
        for (int y = 0; y < _boardHeight; y++)
        {
            for (int x = 0; x < _boardWidth; x++)
            {
                if (_board[y, x] == null)
                {
                    Diamond newDiamond = Instantiate(_diamondPrefabs[UnityEngine.Random.Range(0, _diamondPrefabs.Length)],
                        new Vector3(x, y + _boardHeight, 0), Quaternion.identity, _diamondContainer);
                    sequence.Join(_dropDiamondSO.Drop(newDiamond.gameObject, y));
                    _board[y, x] = newDiamond;
                }
            }
        }

        yield return sequence.Play().WaitForCompletion();
        yield return new WaitForEndOfFrame();
    }

    private IEnumerator ResolveMatches()
    {
        yield return new WaitForSeconds(1f);
        _diamondContainer.gameObject.SetActive(false);
        MessageManager.SendMessage(new Message(GameMessageType.OnMatchResolve, new object[] { _matchDiamondCount }));
        yield return null;
    }

    private IEnumerator ApplyEffect()
    {
        float delay = 1f;
        yield return new WaitForSeconds(delay);
        TurnType currentTurn = TurnManager.Instance.CurrentTurn;
        if (currentTurn == TurnType.EnemyTurn)
        {
            foreach (var item in _matchDiamondCount)
            {
                MessageManager.SendMessage(new Message(GameMessageType.OnEnemyApplyEffect, new object[] { item.Key, item.Value }));
                yield return new WaitForSeconds(delay);
            }
        }
        else
        {
            if (_matchDiamondCount.Count > 0)
            {
                foreach (var item in _matchDiamondCount)
                {
                    MessageManager.SendMessage(new Message(GameMessageType.OnPlayerApplyEffect, new object[] { item.Key, item.Value }));
                    yield return new WaitForSeconds(delay);
                }
            }
        }
    }
    public List<Tuple<GameObject, GameObject>> GenerateValidMoves()
    {
        List<Tuple<GameObject, GameObject>> validMoves = new List<Tuple<GameObject, GameObject>>();
        Vector3[] directions = new Vector3[2]
        {
            Vector3.down, Vector3.right
        };

        Func<Vector3, bool> IsValidPosition = pos =>
            pos.x >= 0 && pos.x < _boardWidth &&
            pos.y >= 0 && pos.y < _boardHeight;

        for (int y = 0; y < _boardHeight; y++)
        {
            for (int x = 0; x < _boardWidth; x++)
            {
                //SWAP CURRENT DIAMOND AND IT'S RIGHT DIAMOND
                //SWAP CURRENT DIAMOND AND IT'S DOWN DIAMOND
                foreach (var direction in directions)
                {
                    Vector3 currentDiamond = new Vector3(x, y);
                    Vector3 neighborDiamond = new Vector3(x + direction.x, y + direction.y);
                    //SWAP
                    if (IsValidPosition(neighborDiamond))
                    {
                        SwapDiamond(currentDiamond, neighborDiamond);
                        if (_matchFinder.FindMatches(_boardWidth, _boardHeight, _board).Count > 0)
                        {
                            GameObject currentDiamondGO = _board[(int)currentDiamond.y, (int)currentDiamond.x].gameObject;
                            GameObject neighborDiamondGO = _board[(int)neighborDiamond.y, (int)neighborDiamond.x].gameObject;
                            validMoves.Add(Tuple.Create(currentDiamondGO, neighborDiamondGO));
                        }
                        //UNDO
                        SwapDiamond(currentDiamond, neighborDiamond);
                    }
                }
            }
        }
        return validMoves;
    }

    private void ResetDiamondCount()
    {
        _matchDiamondCount.Clear();
    }

}
