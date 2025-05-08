using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : Singleton<BoardManager>, IMessageHandle
{

    [SerializeField] private int _boardWidth;
    [SerializeField] private int _boardHeight;
    [SerializeField] private Transform _diamondContainer;
    public int BoardWidth => _boardWidth;
    public int BoardHeight => _boardHeight;
    private Diamond[,] _board;
    public Diamond[,] Board => _board;
    private MatchFinder _matchFinder;
    private BoardProcessor _boardProcessor;
    private HashSet<Diamond> _allMatches = new HashSet<Diamond>();
    void Start()
    {
        _matchFinder = GetComponent<MatchFinder>();
        _boardProcessor = GetComponent<BoardProcessor>();
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
        _boardProcessor.ClearBoard(_board);
        _board = new Diamond[_boardWidth, _boardHeight];
        _boardProcessor.GenerateBoard(_board, _diamondContainer);
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
                ShowDiamondBoard();
                break;
        }
    }

    private IEnumerator ProcessSwapping(GameObject previousDiamond, GameObject currentDiamond)
    {
        yield return _boardProcessor.Swap(previousDiamond, currentDiamond, () =>
        {
            SwapDiamondValue(previousDiamond.transform.position, currentDiamond.transform.position);
        });
        _allMatches = _matchFinder.FindMatches(_boardWidth, _boardHeight, _board);
        if (_allMatches != null && _allMatches.Count > 0)
        {
            //Pause current turn
            MessageManager.SendMessage(new Message(GameMessageType.OnCurrentTurnPaused));
            while (_allMatches != null && _allMatches.Count > 0)
            {
                yield return _boardProcessor.ClearAllMatchDiamond(_allMatches);
                yield return _boardProcessor.CollapseBoard(_board);
                yield return _boardProcessor.RefillBoard(_board, _diamondContainer);
                _allMatches = _matchFinder.FindMatches(_boardWidth, _boardHeight, _board);
            }
            yield return new WaitForSeconds(2f);
            MessageManager.SendMessage(new Message(GameMessageType.OnBoardProcessed));
            HideDiamondBoard();
        }
        else
        {
            yield return _boardProcessor.Swap(previousDiamond, currentDiamond, () =>
            {
                SwapDiamondValue(previousDiamond.transform.position, currentDiamond.transform.position);
                MessageManager.SendMessage(new Message(GameMessageType.OnDiamondSwappedFail));
            });

        }
    }

    private void SwapDiamondValue(Vector3 prev, Vector3 curr)
    {
        Diamond temp = _board[(int)prev.y, (int)prev.x];
        _board[(int)prev.y, (int)prev.x] = _board[(int)curr.y, (int)curr.x];
        _board[(int)curr.y, (int)curr.x] = temp;
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
                foreach (var direction in directions)
                {
                    Vector3 currentDiamond = new Vector3(x, y);
                    Vector3 neighborDiamond = new Vector3(x + direction.x, y + direction.y);
                    //SWAP
                    if (IsValidPosition(neighborDiamond))
                    {
                        SwapDiamondValue(currentDiamond, neighborDiamond);
                        if (_matchFinder.FindMatches(_boardWidth, _boardHeight, _board).Count > 0)
                        {
                            GameObject currentDiamondGO = _board[(int)currentDiamond.y, (int)currentDiamond.x].gameObject;
                            GameObject neighborDiamondGO = _board[(int)neighborDiamond.y, (int)neighborDiamond.x].gameObject;
                            validMoves.Add(Tuple.Create(currentDiamondGO, neighborDiamondGO));
                        }
                        //UNDO
                        SwapDiamondValue(currentDiamond, neighborDiamond);
                    }
                }
            }
        }
        return validMoves;
    }

    public void ShowDiamondBoard()
    {
        _diamondContainer.gameObject.SetActive(true);
    }

    public void HideDiamondBoard()
    {
        _diamondContainer.gameObject.SetActive(false);
    }

}
