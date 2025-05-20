using System;
using System.Collections.Generic;
using System.Xml;
using NUnit.Framework.Interfaces;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "GreedySearch", menuName = "Scriptable Objects/EnemyAIStrategy/GreedySearch")]
public class GreedySearch : AIStrategySO
{
    private Dictionary<DiamondType, int> _greedyScores = new Dictionary<DiamondType, int>();
    public Dictionary<DiamondType, int> GreedyScores => _greedyScores;
    [Header("For board processing")]
    [SerializeField] private int _attackScore;
    [SerializeField] private int _stealScore;
    [SerializeField] private int _shieldScore;
    [SerializeField] private int _ragePointScore;
    [SerializeField] private int _healthPointScore;
    [SerializeField] private int _magicPointScore;
    [SerializeField] private int _magicPointBonus;
    private int _currentMagicPoint;
    private BoardManager _boardManager;

    [Header("For Card Using")]
    [SerializeField] private int _gainCardPoint;
    [SerializeField] private int _attackCardPoint;
    [SerializeField] private int _clearCardPoint;
    private Dictionary<CardType, int> _greedyCardScores = new Dictionary<CardType, int>();

    void OnEnable()
    {
        InitializeGreedyScoresDictionary();
    }

    #region Method for board processing
    private void InitializeGreedyScoresDictionary()
    {
        _greedyScores.Clear();
        _greedyScores.Add(DiamondType.Attack, _attackScore);
        _greedyScores.Add(DiamondType.Steal, _stealScore);
        _greedyScores.Add(DiamondType.Shield, _shieldScore);
        _greedyScores.Add(DiamondType.Rage, _ragePointScore);
        _greedyScores.Add(DiamondType.Health, _healthPointScore);
        _greedyScores.Add(DiamondType.MagicPoint, _currentMagicPoint);

        _greedyCardScores.Clear();
        _greedyCardScores.Add(CardType.GainHPCard, _gainCardPoint);
        _greedyCardScores.Add(CardType.GaimMagicPointCard, _gainCardPoint);
        _greedyCardScores.Add(CardType.GainRagePointCard, _gainCardPoint);
        _greedyCardScores.Add(CardType.AttackCard, _attackCardPoint);
        _greedyCardScores.Add(CardType.ClearCard, _clearCardPoint);
    }

    public override Tuple<GameObject, GameObject> FindBestMove(List<Tuple<GameObject, GameObject>> validMoves)
    {
        Tuple<GameObject, GameObject> bestMove = validMoves[0];
        int maxScore = 0;
        foreach (var move in validMoves)
        {
            int score = CalculateScore(move);
            if (score > maxScore)
            {
                bestMove = move;
                maxScore = score;
            }
        }
        return bestMove;
    }

    public int CalculateScore(Tuple<GameObject, GameObject> move)
    {
        CalculateMagicPoint();
        _boardManager = BoardManager.Instance;
        int score = 0;
        Vector3 currentDiamond = move.Item1.transform.position;
        Vector3 neighborDiamond = move.Item2.transform.position;
        DiamondType[,] clonedData = _boardManager.BoardData;

        _boardManager.SwapDiamondType(currentDiamond, neighborDiamond, clonedData);
        HashSet<Vector2Int> _allMatches = _boardManager.MatchFinder.FindMatches(clonedData);
        while (_allMatches.Count > 0)
        {
            foreach (var item in _allMatches)
            {
                score += _greedyScores[clonedData[item.y, item.x]];
            }
            _boardManager.BoardProcessor.ClearDiamondData(_allMatches, clonedData);
            _boardManager.BoardProcessor.CollapseBoardData(clonedData);
            _allMatches = _boardManager.MatchFinder.FindMatches(clonedData);
        }
        Debug.Log(score);
        return score;
    }

    public int FindBestRow()
    {
        CalculateMagicPoint();
        _boardManager = BoardManager.Instance;
        DiamondType[,] clonedData = _boardManager.BoardData;
        int maxScore = 0;
        int res = 0;
        for (int row = 0; row < clonedData.GetLength(0); row++)
        {
            int score = 0;
            clonedData = _boardManager.BoardData;
            for (int x = 0; x < clonedData.GetLength(1); x++)
            {
                score += _greedyScores[clonedData[row, x]];
                clonedData[row, x] = DiamondType.None;
            }
            _boardManager.BoardProcessor.CollapseBoardData(clonedData);
            HashSet<Vector2Int> _allMatches = _boardManager.MatchFinder.FindMatches(clonedData);
            while (_allMatches.Count > 0)
            {
                foreach (var item in _allMatches)
                {
                    score += _greedyScores[clonedData[item.y, item.x]];
                }
                _boardManager.BoardProcessor.ClearDiamondData(_allMatches, clonedData);
                _boardManager.BoardProcessor.CollapseBoardData(clonedData);
                _allMatches = _boardManager.MatchFinder.FindMatches(clonedData);
            }
            if (score > maxScore)
            {
                maxScore = score;
                res = row;
            }
        }
        return res;
    }

    public int FindBestColumn()
    {
        CalculateMagicPoint();
        _boardManager = BoardManager.Instance;
        DiamondType[,] clonedData = _boardManager.BoardData;
        int maxScore = 0;
        int res = 0;
        for (int col = 0; col < clonedData.GetLength(1); col++)
        {
            int score = 0;
            clonedData = _boardManager.BoardData;
            for (int y = 0; y < clonedData.GetLength(1); y++)
            {
                score += _greedyScores[clonedData[y, col]];
                clonedData[y, col] = DiamondType.None;
            }
            _boardManager.BoardProcessor.CollapseBoardData(clonedData);
            HashSet<Vector2Int> _allMatches = _boardManager.MatchFinder.FindMatches(clonedData);
            while (_allMatches.Count > 0)
            {
                foreach (var item in _allMatches)
                {
                    score += _greedyScores[clonedData[item.y, item.x]];
                }
                _boardManager.BoardProcessor.ClearDiamondData(_allMatches, clonedData);
                _boardManager.BoardProcessor.CollapseBoardData(clonedData);
                _allMatches = _boardManager.MatchFinder.FindMatches(clonedData);
            }
            if (score > maxScore)
            {
                maxScore = score;
                res = col;
            }
        }
        return res;

    }
    public Tuple<int, int> FindBestCrossPoint()
    {
        Tuple<int, int> res = Tuple.Create(0, 0);
        CalculateMagicPoint();
        _boardManager = BoardManager.Instance;
        DiamondType[,] clonedData = _boardManager.BoardData;
        int maxScore = 0;
        List<int> rowScores = new List<int>();
        List<int> colScores = new List<int>();
        for (int row = 0; row < clonedData.GetLength(0); row++)
        {
            int rowScore = 0;
            for (int x = 0; x < clonedData.GetLength(1); x++)
            {
                rowScore += _greedyScores[clonedData[row, x]];
            }
            rowScores.Add(rowScore);

        }

        for (int col = 0; col < clonedData.GetLength(1); col++)
        {
            int colScore = 0;
            for (int y = 0; y < clonedData.GetLength(0); y++)
            {
                colScore += _greedyScores[clonedData[y, col]];
            }
            colScores.Add(colScore);
        }

        for (int y = 0; y < clonedData.GetLength(0); y++)
        {
            for (int x = 0; x < clonedData.GetLength(1); x++)
            {
                int score = 0;
                clonedData = _boardManager.BoardData;
                score += rowScores[y] + colScores[x] - _greedyScores[clonedData[y, x]];
                _boardManager.BoardProcessor.ClearCrossBoardData(clonedData, y, x);
                _boardManager.BoardProcessor.CollapseBoardData(clonedData);
                HashSet<Vector2Int> _allMatches = _boardManager.MatchFinder.FindMatches(clonedData);
                while (_allMatches.Count > 0)
                {
                    foreach (var item in _allMatches)
                    {
                        score += _greedyScores[clonedData[item.y, item.x]];
                    }
                    _boardManager.BoardProcessor.ClearDiamondData(_allMatches, clonedData);
                    _boardManager.BoardProcessor.CollapseBoardData(clonedData);
                    _allMatches = _boardManager.MatchFinder.FindMatches(clonedData);
                }

                if (score > maxScore)
                {
                    maxScore = score;
                    res = Tuple.Create(y, x);
                }
            }
            Debug.Log(res);
        }
        return res;
    }
    private void CalculateMagicPoint()
    {
        int cardRemaining = EnemyController.Instance.GetCardRequiringMagicPoint();
        if (cardRemaining > 0)
        {
            _currentMagicPoint = _magicPointScore + _magicPointBonus;
        }
        else _currentMagicPoint = _magicPointScore;
    }


    #endregion


    #region Method for card using
    public override CardData FindBestCard(List<CardData> usableCardList)
    {
        CardData bestCard = null;
        int maxScore = 0;
        foreach (var cardData in usableCardList)
        {
            int score = _greedyCardScores[cardData.CardType];
            if (score > maxScore)
            {
                maxScore = score;
                bestCard = cardData;
            }
        }
        return bestCard;
    }
    #endregion

}
