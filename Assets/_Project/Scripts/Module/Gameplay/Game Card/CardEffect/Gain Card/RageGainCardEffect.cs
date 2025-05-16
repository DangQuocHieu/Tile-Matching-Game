using System.Collections;
using UnityEngine;
[CreateAssetMenu(fileName = "RageGain", menuName = "Scriptable Objects/Card Effect/RageGain")]
public class RageGainCardEffect : CardEffectSO
{
    [SerializeField] private int _effectValue;

    public override IEnumerator Activate()
    {
        MessageManager.SendMessage(new Message(GameMessageType.OnApplyGainCard, new object[] { DiamondType.Rage, _effectValue }));
        yield return new WaitForSeconds(_applyDuration);
        BattleManager.Instance.CurrentUnit.StatHandler.AddRagePoint(_effectValue);
        MessageManager.SendMessage(new Message(GameMessageType.OnApplyCardEffectEnd));
    }
}