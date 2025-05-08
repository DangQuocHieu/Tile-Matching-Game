using System.Collections;
using DG.Tweening;
using UnityEngine;

public abstract class AttackStrategySO : ScriptableObject, IAttackStrategy
{
    public abstract IEnumerator Execute(GameUnit attacker, GameUnit target, TweenCallback callback = null);
}
