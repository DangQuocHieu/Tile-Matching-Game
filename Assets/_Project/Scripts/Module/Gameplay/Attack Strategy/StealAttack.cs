using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "StealAttack", menuName = "Scriptable Objects/AttackStrategy/StealAttack")]
public class StealAttack : AttackStrategySO
{   
    public override IEnumerator Execute(GameUnit attacker, GameUnit target)
    {
        List<DiamondType> typesToSteal = target.StatHandler.GetStealableTypes();
        DiamondType typeToSteal = typesToSteal[Random.Range(0, typesToSteal.Count)];
        int valueToSteal = BattleManager.Instance.GetStealMatchedCount() * attacker.StatHandler.Stat.BaseStealPoint;
        int targetShieldPoint = target.StatHandler.CurrentShieldPoint;
        if(targetShieldPoint != 0)
        {
            valueToSteal = 0;
            target.StatHandler.ResetShieldPoint();
        }
        yield return target.StatHandler.OnStolen(typeToSteal, valueToSteal);
        target.AnimationHandler.SetHurtState();
        yield return attacker.StatHandler.ApplyEffect(typeToSteal, valueToSteal);
        target.AnimationHandler.SetIdleState();
    }
}
