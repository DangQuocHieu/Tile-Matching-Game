using System.Collections;
using UnityEngine;
[CreateAssetMenu(fileName = "ClearColumn", menuName = "Scriptable Objects/Card Effect/ClearColumn")]
public class ClearColumnCardEffect : CardEffectSO
{
    [SerializeField] private Side _playerSide;
    
    public override IEnumerator Activate()
    {
        Side currentSide = TurnManager.Instance.CurrentSide;
        if(currentSide == _playerSide)
        {
            PlayerController.Instance.ResetDiamond();
            yield return new WaitUntil(() => PlayerController.Instance.CurrentDiamond != default);
            PlayerController.Instance.DisableControl();
            DiamondHighlight.Instance.UnHighlight();
            BoardManager.Instance.ClearEntireCol(PlayerController.Instance.CurrentDiamond);
        }
    }
}
