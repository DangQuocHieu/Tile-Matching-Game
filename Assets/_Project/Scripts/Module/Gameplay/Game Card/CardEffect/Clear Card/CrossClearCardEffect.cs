using System.Collections;
using UnityEngine;
[CreateAssetMenu(fileName = "CrossClear", menuName = "Scriptable Objects/Card Effect/CrossClear")]
public class CrossClearCardEffect : CardEffectSO
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
            BoardManager.Instance.CrossClear(PlayerController.Instance.CurrentDiamond);
        }
    }
    public override void OnComplete(GameObject gameObject)
    {
        Destroy(gameObject);
    }
}
