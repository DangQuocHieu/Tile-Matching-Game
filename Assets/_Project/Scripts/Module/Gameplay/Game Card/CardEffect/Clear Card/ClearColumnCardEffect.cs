using System.Collections;
using UnityEngine;
[CreateAssetMenu(fileName = "ClearColumn", menuName = "Scriptable Objects/Card Effect/ClearColumn")]
public class ClearColumnCardEffect : CardEffectSO
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
            yield return BoardManager.Instance.ClearEntireCol(PlayerController.Instance.CurrentDiamond);
        }
        else
        {
            int bestColumn = EnemyController.Instance.GreedySearch.FindBestColumn();
            GameObject diamond = BoardManager.Instance.GetRandomDiamondInCol(bestColumn);
            yield return BoardManager.Instance.ClearEntireCol(diamond);
        }
    }
}
