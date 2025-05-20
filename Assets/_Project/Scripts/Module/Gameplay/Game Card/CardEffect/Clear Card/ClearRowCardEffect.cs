using System.Collections;
using UnityEngine;
[CreateAssetMenu(fileName = "ClearRow", menuName = "Scriptable Objects/Card Effect/ClearRow")]
public class ClearRowCardEffect : CardEffectSO
{
    [SerializeField] private Side _playerSide;

    public override IEnumerator Activate()
    {
        Side currentSide = TurnManager.Instance.CurrentSide;
        if (currentSide == _playerSide)
        {
            yield return new WaitUntil(() => PlayerController.Instance.CurrentDiamond != null);
            PlayerController.Instance.DisableControl();
            DiamondHighlight.Instance.UnHighlight();
            yield return BoardManager.Instance.ClearEntireRow(PlayerController.Instance.CurrentDiamond);
        }
        else
        {
            int bestRow = EnemyController.Instance.GreedySearch.FindBestRow();
            GameObject diamond = BoardManager.Instance.GetRandomDiamondInRow(bestRow);
            yield return BoardManager.Instance.ClearEntireRow(diamond);
        }
    }
}
