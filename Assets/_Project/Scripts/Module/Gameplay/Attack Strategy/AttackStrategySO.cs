using System.Collections;
using DG.Tweening;
using UnityEngine;

public abstract class AttackStrategySO : ScriptableObject, IAttackStrategy
{
    public abstract IEnumerator Execute(GameObject attacker, GameObject target, TweenCallback callback = null);
}
