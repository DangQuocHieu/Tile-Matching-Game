using System.Collections;
using DG.Tweening;
using UnityEngine;

public class UnitAttackHandler : MonoBehaviour
{
    [SerializeField] AttackStrategySO _meleeAttack;

    public IEnumerator Attack(GameUnit attacker, GameUnit target, TweenCallback callback)
    {
        yield return _meleeAttack.Execute(attacker, target, callback);
    }

}
