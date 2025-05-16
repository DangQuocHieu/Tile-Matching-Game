using System.Collections;
using UnityEngine;
[CreateAssetMenu(fileName = "HealGain", menuName = "Scriptable Objects/Card Effect/HealGain")]
public class HealthGainCardEffect : CardEffectSO
{
    [SerializeField] private int _effectValue;
    public override IEnumerator Activate()
    {
        MessageManager.SendMessage(new Message(GameMessageType.OnApplyGainCard, new object[] { DiamondType.Health, _effectValue }));
        yield return new WaitForSeconds(_applyDuration);
        BattleManager.Instance.CurrentUnit.StatHandler.AddHealthPoint(_effectValue);
        MessageManager.SendMessage(new Message(GameMessageType.OnApplyCardEffectEnd));
    }
}
