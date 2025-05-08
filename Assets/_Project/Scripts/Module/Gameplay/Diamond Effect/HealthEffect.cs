using System.Collections;
using UnityEngine;

public class HealthEffect : IEffect
{
    public IEnumerator ApplyEffect(UnitStatHandler statHandler, int value)
    {
        yield return statHandler.AddHealthPoint(value);
    }
}
