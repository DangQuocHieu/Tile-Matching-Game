using UnityEngine;

public class ManaEffect : IEffect
{
    public void ApplyEffect(UnitStatHandler statHandler, int value)
    {
        statHandler.AddManaPoint(value);
    }
}
