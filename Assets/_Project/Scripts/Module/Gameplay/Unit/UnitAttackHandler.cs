using System.Collections;
using DG.Tweening;
using UnityEngine;

public class UnitAttackHandler : MonoBehaviour
{
    [SerializeField] AttackStrategySO _meleeAttack;
    [SerializeField] AttackStrategySO _stealAttack;

    public IEnumerator Attack(GameUnit attacker, GameUnit target)
    {
        yield return _meleeAttack.Execute(attacker, target);
    }

    public IEnumerator Steal(GameUnit attaker, GameUnit target)
    {
        yield return _stealAttack.Execute(attaker, target);
    }

}
