using UnityEngine;

public class HealthEffect : IEffect
{
    public void ApplyEffect(UnitStatHandler statHandler, int value)
    {
        statHandler.AddHealthPoint(value);
    }
}
