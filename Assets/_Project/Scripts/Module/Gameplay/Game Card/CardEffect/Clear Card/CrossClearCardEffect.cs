using System;
using System.Collections;
using UnityEngine;
[CreateAssetMenu(fileName = "CrossClear", menuName = "Scriptable Objects/Card Effect/CrossClear")]
public class CrossClearCardEffect : CardEffectSO
{
    [SerializeField] private Side _playerSide;

    public override IEnumerator Activate()
    {
        Side currentSide = TurnManager.Instance.CurrentSide;
        if (currentSide == _playerSide)
        {
            yield return new WaitUntil(() => PlayerController.Instance.CurrentDiamond != default);
            PlayerController.Instance.DisableControl();
            DiamondHighlight.Instance.UnHighlight();
            yield return BoardManager.Instance.CrossClear(PlayerController.Instance.CurrentDiamond);
        }
        else
        {
            Tuple<int,int> bestCross = EnemyController.Instance.GreedySearch.FindBestCrossPoint();
            GameObject diamond = BoardManager.Instance.GetDiamondInCross(bestCross.Item1, bestCross.Item2);
            yield return BoardManager.Instance.CrossClear(diamond);
        }
    }
}
