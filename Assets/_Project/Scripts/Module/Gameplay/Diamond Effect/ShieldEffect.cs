using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "ShieldEffect", menuName = "Scriptable Objects/DiamondEffect/ShieldEffect")]
public class ShieldEffect : GameEffectSO
{
    public override IEnumerator Execute(int value)
    {
        yield return new WaitForSeconds(_applyDuration);
        BattleManager.Instance.CurrentUnit.StatHandler.AddShieldPoint(value);
    }
    public override IEnumerator Execute(UnitStatHandler handler, int value)
    {
        yield return new WaitForSeconds(_applyDuration);
        handler.AddShieldPoint(value);
    }
}
