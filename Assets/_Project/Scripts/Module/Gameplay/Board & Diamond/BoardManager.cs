using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
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

    private DiamondType[,] _boardData;
    public DiamondType[,] BoardData {
        get {
            UpdateBoardData();
            return _boardData;
        }
    }
    private MatchFinder _matchFinder;
    public MatchFinder MatchFinder => _matchFinder;
    private BoardProcessor _boardProcessor;
    public BoardProcessor BoardProcessor => _boardProcessor;
    private HashSet<Vector2Int> _allMatches = new HashSet<Vector2Int>();

    void Start()
    {
        _boardData = new DiamondType[_boardWidth,_boardHeight]; 
        _matchFinder = GetComponent<MatchFinder>();
        _boardProcessor = GetComponent<BoardProcessor>();
        GenerateBoard();
    }

    void OnEnable()
    {
        MessageManager.AddSubscriber(GameMessageType.OnDiamondSwapped, this);
        MessageManager.AddSubscriber(GameMessageType.OnTurnStartDelay, this);
        MessageManager.AddSubscriber(GameMessageType.OnApplyEffectStart, this);
        MessageManager.AddSubscriber(GameMessageType.OnApplyCardEffectEnd, this);
        MessageManager.AddSubscriber(GameMessageType.OnApplyGainCard, this);
    }

    void OnDisable()
    {
        MessageManager.RemoveSubscriber(GameMessageType.OnDiamondSwapped, this);
        MessageManager.RemoveSubscriber(GameMessageType.OnTurnStartDelay, this);
        MessageManager.RemoveSubscriber(GameMessageType.OnApplyEffectStart, this);
        MessageManager.RemoveSubscriber(GameMessageType.OnApplyCardEffectEnd, this);
        MessageManager.RemoveSubscriber(GameMessageType.OnApplyGainCard, this);
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
            case GameMessageType.OnApplyCardEffectEnd:
                ShowDiamondBoard();
                break;
            case GameMessageType.OnApplyGainCard:
                HideDiamondBoard();
                break;

        }
    }


    private IEnumerator ProcessSwapping(GameObject previousDiamond, GameObject currentDiamond)
    {
        yield return _boardProcessor.Swap(previousDiamond, currentDiamond, () =>
        {
            SwapDiamondValue(previousDiamond.transform.position, currentDiamond.transform.position, _board);
        });
        UpdateBoardData();
        _allMatches = _matchFinder.FindMatches(_boardData);
        if (_allMatches != null && _allMatches.Count > 0)
        {
            //Pause current turn
            TurnManager.Instance.PauseCurrentTurn();
            //
            yield return ProcessBoard();
        }
        else
        {
            yield return _boardProcessor.Swap(previousDiamond, currentDiamond, () =>
            {
                SwapDiamondValue(previousDiamond.transform.position, currentDiamond.transform.position, _board);
                MessageManager.SendMessage(new Message(GameMessageType.OnDiamondSwappedFail));
            });

        }
    }
    public IEnumerator ProcessBoard()
    {
        MessageManager.SendMessage(new Message(GameMessageType.OnProcessBoardStart));
        while (_allMatches != null && _allMatches.Count > 0)
        {
            yield return _boardProcessor.ClearDiamond(_allMatches, _board);
            yield return _boardProcessor.CollapseBoard(_board);
            yield return _boardProcessor.RefillBoard(_board, _diamondContainer);
            UpdateBoardData();
            _allMatches = _matchFinder.FindMatches(_boardData);

        }
        yield return SendBoardProcessedMessage();
        HideDiamondBoard();
    }

    private IEnumerator SendBoardProcessedMessage()
    {
        yield return new WaitForSeconds(2f);
        MessageManager.SendMessage(new Message(GameMessageType.OnBoardProcessed));
    }

    public void SwapDiamondValue(Vector3 prev, Vector3 curr, Diamond[,] board)
    {
        Diamond temp = board[(int)prev.y, (int)prev.x];
        board[(int)prev.y, (int)prev.x] = board[(int)curr.y, (int)curr.x];
        board[(int)curr.y, (int)curr.x] = temp;
    }

    public List<Tuple<GameObject, GameObject>> GenerateValidMoves()
    {
        UpdateBoardData();
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
                        SwapDiamondType(currentDiamond, neighborDiamond, _boardData);
                        if (_matchFinder.FindMatches(_boardData).Count > 0)
                        {
                            GameObject currentDiamondGO = _board[(int)currentDiamond.y, (int)currentDiamond.x].gameObject;
                            GameObject neighborDiamondGO = _board[(int)neighborDiamond.y, (int)neighborDiamond.x].gameObject;
                            validMoves.Add(Tuple.Create(currentDiamondGO, neighborDiamondGO));
                        }
                        //UNDO
                        SwapDiamondType(currentDiamond, neighborDiamond, _boardData);
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

    //Reuse for clear entire row and clear entire col method
    public void ClearEntireRow(GameObject diamond)
    {
        int row = (int)diamond.transform.position.y;
        HashSet<Vector2Int> diamondsInRow = new HashSet<Vector2Int>();
        for (int x = 0; x < _board.GetLength(1); x++)
        {
            diamondsInRow.Add(new Vector2Int(x, row));
        }
        StartCoroutine(ClearHandler(diamondsInRow));
    }

    public void ClearEntireCol(GameObject diamond)
    {
        int col = (int)diamond.transform.position.x;
        HashSet<Vector2Int> diamondsInCol = new HashSet<Vector2Int>();
        for (int y = 0; y < _board.GetLength(0); y++)
        {
            diamondsInCol.Add(new Vector2Int(col, y));
        }
        StartCoroutine(ClearHandler(diamondsInCol));
    }

    public void CrossClear(GameObject diamond)
    {
        int row = (int)diamond.transform.position.y;
        int col = (int)diamond.transform.position.x;
        HashSet<Vector2Int> diamondsInCross = new HashSet<Vector2Int>();
        for (int x = 0; x < _board.GetLength(1); x++)
        {
            diamondsInCross.Add(new Vector2Int(x, row));
        }
        for (int y = 0; y < _board.GetLength(0); y++)
        {
            diamondsInCross.Add(new Vector2Int(col, y));
        }
        StartCoroutine(ClearHandler(diamondsInCross));
    }

    private IEnumerator ClearHandler(HashSet<Vector2Int> diamondsToClear)
    {
        TurnManager.Instance.PauseCurrentTurn();
        MessageManager.SendMessage(new Message(GameMessageType.OnProcessBoardStart));
        yield return _boardProcessor.ClearDiamond(diamondsToClear, _board);
        yield return _boardProcessor.CollapseBoard(_board);
        yield return _boardProcessor.RefillBoard(_board, _diamondContainer);
        UpdateBoardData();
        _allMatches = _matchFinder.FindMatches(_boardData);
        if (_allMatches != null && _allMatches.Count > 0)
        {
            yield return ProcessBoard();
        }

        else
        {
            yield return SendBoardProcessedMessage();
            HideDiamondBoard();
        }
    }

    private void UpdateBoardData()
    {
        for(int y = 0; y < _board.GetLength(0); y++)
        {
            for(int x = 0; x < _board.GetLength(1); x++)
            {
                _boardData[y,x] = _board[y, x].DiamondType;
            }
        }
    }
    #region Method for find best move
    public void SwapDiamondType(Vector3 prev, Vector3 curr, DiamondType[,] boardData)
    {
        DiamondType temp = boardData[(int)prev.y, (int)prev.x];
        boardData[(int)prev.y, (int)prev.x] = boardData[(int)curr.y, (int)curr.x];
        boardData[(int)curr.y, (int)curr.x] = temp;
    }
    #endregion
}
