using UnityEngine;

public abstract class UnitStatHandler : MonoBehaviour
{
    [SerializeField] protected UnitStat _stat;
    [SerializeField] protected int _currentHealthPoint;
    [SerializeField] protected int _currentManaPoint;
    [SerializeField] protected int _currentRagePoint;
    [SerializeField] protected int _shieldPoint;
    [SerializeField] protected int _attackDamage;

    protected virtual void Awake()
    {
        SetUpStat();
    }

    protected void SetUpStat()
    {
        _currentHealthPoint = _stat.MaxHealthPoint;
        _currentManaPoint = 0;
        _currentRagePoint = 0;
        _shieldPoint = 0;
        _attackDamage = _stat.BaseDamage;
    }

    public abstract void ApplyEffect(DiamondType type, int counter);
    protected void AddManaPoint(int counter)
    {
        _currentManaPoint += counter * _stat.ManaIncrease;
    }

    protected void AddRagePoint(int counter)
    {
        _currentRagePoint += counter * _stat.RageIncrease;
    }

    protected void AddHealthPoint(int counter)
    {
        _currentHealthPoint += counter * _stat.HealthIncrease;
    }

    protected void AddShieldPoint(int counter)
    {
        _shieldPoint += counter * _stat.ShieldIncrease;
    }
}
