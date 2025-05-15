using System.Collections;
using UnityEngine;
public abstract class GameEffectSO : ScriptableObject, IGameEffect
{
    [SerializeField] protected float _applyDuration;
    [SerializeField] protected float _delay = 0.5f;
    public abstract IEnumerator Execute(int value);
    public abstract IEnumerator Execute(UnitStatHandler handler, int value);
}
