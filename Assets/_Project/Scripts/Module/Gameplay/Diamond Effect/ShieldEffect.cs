using System.Collections;

public class ShieldEffect : IEffect
{
    public IEnumerator ApplyEffect(UnitStatHandler statHandler, int value)
    {
        yield return statHandler.AddShieldPoint(value);
    }
}
