using System.Collections;
using UnityEngine;
[CreateAssetMenu(fileName = "MagicPointGain", menuName = "Scriptable Objects/Card Effect/MagicPointGain")]
public class MagicPointGainCardEffect : CardEffectSO
{
    [SerializeField] private int _effectValue;
    public override IEnumerator Activate()
    {
        MessageManager.SendMessage(new Message(GameMessageType.OnApplyGainCard, new object[] { DiamondType.MagicPoint, _effectValue }));
        yield return new WaitForSeconds(_applyDuration);
        BattleManager.Instance.CurrentUnit.StatHandler.AddMagicPoint(_effectValue);
        MessageManager.SendMessage(new Message(GameMessageType.OnApplyCardEffectEnd));
    }
}
