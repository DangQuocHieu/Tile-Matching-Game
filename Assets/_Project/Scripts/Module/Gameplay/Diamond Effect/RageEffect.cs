using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "RageEffect", menuName = "Scriptable Objects/DiamondEffect/RageEffect")]
public class RageEffect : GameEffectSO
{
    public override IEnumerator Execute(int value)
    {
        yield return new WaitForSeconds(_applyDuration);
        BattleManager.Instance.CurrentUnit.StatHandler.AddRagePoint(value);
    }
    public override IEnumerator Execute(UnitStatHandler handler, int value)
    {
        yield return new WaitForSeconds(_applyDuration);
        handler.AddRagePoint(value);
    }
}
