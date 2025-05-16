using System.Collections;
using UnityEngine;
[CreateAssetMenu(fileName = "ClearRow", menuName = "Scriptable Objects/Card Effect/ClearRow")]
public class ClearRowCardEffect : CardEffectSO
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
            BoardManager.Instance.ClearEntireRow(PlayerController.Instance.CurrentDiamond);
        }
    }
}
