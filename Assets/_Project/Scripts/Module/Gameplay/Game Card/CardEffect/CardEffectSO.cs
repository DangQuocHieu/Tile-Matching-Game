using System.Collections;
using UnityEngine;

public abstract class CardEffectSO : ScriptableObject, ICardEffect
{
    [SerializeField] protected float _applyDuration;
    public abstract IEnumerator Activate();
    
}
