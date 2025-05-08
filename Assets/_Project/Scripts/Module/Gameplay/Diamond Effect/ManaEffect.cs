using System.Collections;
using UnityEngine;

public class ManaEffect : IEffect
{
    public IEnumerator ApplyEffect(UnitStatHandler statHandler, int value)
    {
        yield return statHandler.AddManaPoint(value);
    }
}
