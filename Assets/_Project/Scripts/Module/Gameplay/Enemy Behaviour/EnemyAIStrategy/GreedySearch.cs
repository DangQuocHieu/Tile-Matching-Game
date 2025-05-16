using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "GreedySearch", menuName = "Scriptable Objects/EnemyAIStrategy/GreedySearch")]
public class GreedySearch : AIStrategySO
{
    private Dictionary<DiamondType, int> _greedyScores = new Dictionary<DiamondType, int>();
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
        _greedyCardScores.Add(CardType.GainCard, _gainCardPoint);
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
            int score = CalculateCardScore(cardData);
            if (score > maxScore)
            {
                maxScore = score;
                bestCard = cardData;
            }
        }
        return bestCard;
    }
    public int CalculateCardScore(CardData cardData)
    {
        return _greedyCardScores[cardData.CardType];
    }
    #endregion


}
