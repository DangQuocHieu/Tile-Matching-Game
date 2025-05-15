using UnityEngine;

[CreateAssetMenu(fileName = "UnitStat", menuName = "Scriptable Objects/UnitStat")]
public class UnitStat : ScriptableObject
{
    [SerializeField] private string _unitId;
    public string UnitId => _unitId;
    [Header("Max Stats")]
    [SerializeField] private int _maxHealthPoint;
    public int MaxHealthPoint => _maxHealthPoint;
    [SerializeField] private int _maxManaPoint;
    public int MaxManaPoint => _maxManaPoint;
    [SerializeField] private int _maxRagePoint;
    public int MaxRagePoint => _maxRagePoint;
    
    //Base properties per diamond matched
    [Header("Gain Per Diamond")]
    [SerializeField] private int _baseHealthPoint;
    public int BaseHealthPoint => _baseHealthPoint;
    [SerializeField] private int _baseMagicPoint;
    public int BaseMagicPoint => _baseMagicPoint;
    [SerializeField] private int _baseRagePoint;
    public int BaseRagePoint => _baseRagePoint;
    [SerializeField] private int _baseDamage;
    public int BaseDamage => _baseDamage;
    [SerializeField] private int _baseShieldPoint;
    public int BaseShieldPoint => _baseShieldPoint;
    [SerializeField] private int _baseStealPoint;
    public int BaseStealPoint => _baseStealPoint;

    [Header("Another")]
    [SerializeField] private int _ragePointToIncreaseDamage;
    public int RagePointToIncreaseDamage => _ragePointToIncreaseDamage;
    [SerializeField] private float _damageScale = 1.5f;
    public float DamageScale => _damageScale;
}
