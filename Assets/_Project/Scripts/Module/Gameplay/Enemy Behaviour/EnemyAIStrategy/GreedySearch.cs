using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GreedySearch", menuName = "Scriptable Objects/EnemyAIStrategy/GreedySearch")]
public class GreedySearch : AIStrategySO
{
    private Dictionary<DiamondType, int> _greedyScores = new Dictionary<DiamondType, int>();
    [SerializeField] private int _attackScore;
    [SerializeField] private int _stealScore;
    [SerializeField] private int _shieldScore;
    [SerializeField] private int _ragePointScore;
    [SerializeField] private int _healthPointScore;
    [SerializeField] private int _magicPointScore;
    private BoardManager _boardManager;
    void OnEnable()
    {
        InitializeGreedyScoresDictionary();
    }

    private void InitializeGreedyScoresDictionary()
    {
        _greedyScores.Clear();
        _greedyScores.Add(DiamondType.Attack, _attackScore);
        _greedyScores.Add(DiamondType.Steal, _stealScore);
        _greedyScores.Add(DiamondType.Shield, _shieldScore);
        _greedyScores.Add(DiamondType.Rage, _ragePointScore);
        _greedyScores.Add(DiamondType.Health, _healthPointScore);
        _greedyScores.Add(DiamondType.MagicPoint, _magicPointScore);
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
}
