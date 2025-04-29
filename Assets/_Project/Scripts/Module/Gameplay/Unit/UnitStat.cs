using UnityEngine;

[CreateAssetMenu(fileName = "UnitStat", menuName = "Scriptable Objects/UnitStat")]
public class UnitStat : ScriptableObject
{
    [SerializeField] private int _maxHealthPoint;
    public int MaxHealthPoint => _maxHealthPoint;
    [SerializeField] private int _maxManaPoint;
    public int MaxManaPoint => _maxManaPoint;
    [SerializeField] private int _maxRagePoint;
    public int MaxRagePoint => _maxRagePoint;
    [SerializeField] private int _maxShieldPoint;
    public int MaxShieldPoint => _maxShieldPoint;
    
    //Base value when match 1 diamonds.
    [SerializeField] private int _baseDamage;
    public int BaseDamage => _baseDamage;
    [SerializeField] private int _manaIncrease;
    public int ManaIncrease => _manaIncrease;
    [SerializeField] private int _rageIncrease;
    public int RageIncrease => _rageIncrease;
    [SerializeField] private int _shieldIncrease;
    public int ShieldIncrease => _shieldIncrease;

    [SerializeField] private int _healthIncrease;
    public int HealthIncrease => _healthIncrease;

}
