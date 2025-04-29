using System.Collections;
using DG.Tweening;
using UnityEngine;

public interface IAttackStrategy
{
    public IEnumerator Execute(GameObject attacker, GameObject target, TweenCallback callback = null);
}

