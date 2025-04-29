public class ShieldEffect : IEffect
{
    public void ApplyEffect(UnitStatHandler statHandler, int value)
    {
        statHandler.AddShieldPoint(value);
    }
}
