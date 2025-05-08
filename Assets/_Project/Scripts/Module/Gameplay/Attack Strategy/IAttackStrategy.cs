using System.Collections;
using DG.Tweening;
using UnityEngine;

public interface IAttackStrategy
{
    public IEnumerator Execute(GameUnit attacker, GameUnit target, TweenCallback callback = null);
}

