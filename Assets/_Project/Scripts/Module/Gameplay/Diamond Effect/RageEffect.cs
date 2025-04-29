using UnityEngine;

public class RageEffect : IEffect
{
    public void ApplyEffect(UnitStatHandler statHandler, int value)
    {
        statHandler.AddRagePoint(value);
    }
}
