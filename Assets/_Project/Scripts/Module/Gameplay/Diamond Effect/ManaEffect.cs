using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "ManaEffect", menuName = "Scriptable Objects/DiamondEffect/ManaEffect")]
public class ManaEffect : GameEffectSO
{
    public override IEnumerator Execute(int value)
    {
        yield return new WaitForSeconds(_applyDuration);
        BattleManager.Instance.CurrentUnit.StatHandler.AddMagicPoint(value);
    }

    public override IEnumerator Execute(UnitStatHandler handler, int value)
    {
        yield return new WaitForSeconds(_applyDuration);
        handler.AddMagicPoint(value);
    }
}
