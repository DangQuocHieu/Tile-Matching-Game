using System.Collections;
using UnityEngine;

public class RageEffect : IEffect
{
    public IEnumerator ApplyEffect(UnitStatHandler statHandler, int value)
    {
        yield return statHandler.AddRagePoint(value);
    }
}
