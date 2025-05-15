using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "HealthEffect", menuName = "Scriptable Objects/DiamondEffect/HealthEffect")]
public class HealthEffect : GameEffectSO 
{
    public override IEnumerator Execute(int value)
    {
        yield return new WaitForSeconds(_applyDuration);
        BattleManager.Instance.CurrentUnit.StatHandler.AddHealthPoint(value);
    }
    public override IEnumerator Execute(UnitStatHandler handler, int value)
    {
        yield return new WaitForSeconds(_applyDuration);
        handler.AddHealthPoint(value);
    }
}
