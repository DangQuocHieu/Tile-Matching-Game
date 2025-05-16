using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class UnitStatHandler : MonoBehaviour
{
    [Header("Stat SO")]
    [SerializeField] private UnitStat _stat;
    public UnitStat Stat => _stat;

    [Header("Stat")]
    [SerializeField] private int _currentHealthPoint;
    public int CurrentHealthPoint => _currentHealthPoint;
    [SerializeField] private int _currentMagicPoint;
    public int CurrentMagicPoint => _currentMagicPoint;
    [SerializeField] private int _currentRagePoint;
    public int CurrentRagePoint => _currentRagePoint;
    [SerializeField] private int _currentShieldPoint;
    public int CurrentShieldPoint => _currentShieldPoint;
    [Header("For effect dictionary")]
    [SerializeField] private DiamondType[] _diamondTypes;
    [SerializeField] private GameEffectSO[] _diamondEffects;
    private Dictionary<DiamondType, GameEffectSO> _effectDictionary = new Dictionary<DiamondType, GameEffectSO>();
    private Dictionary<DiamondType, int> _baseValue = new Dictionary<DiamondType, int>();
    // [SerializeField] private GameObject _shieldIcon;

    private UnitAnimationHandler _animationHandler;

    void Start()
    {
        _animationHandler = GetComponent<UnitAnimationHandler>();
        InitializeStat();
        InitializeEffectDictionary();
        InitializeBaseValueDictionary();
        // InitShieldIconPosition();
    }

    void Update()
    {
        // DisplayShieldIcon();
    }
    private void InitializeStat()
    {
        _currentHealthPoint = _stat.MaxHealthPoint;
        _currentMagicPoint = 0;
        _currentRagePoint = 0;
    }

    private void InitializeEffectDictionary()
    {
        for (int i = 0; i < _diamondTypes.Length; i++)
        {
            _effectDictionary.Add(_diamondTypes[i], _diamondEffects[i]);
        }
    }

    private void InitializeBaseValueDictionary()
    {
        _baseValue.Add(DiamondType.Health, _stat.BaseHealthPoint);
        _baseValue.Add(DiamondType.MagicPoint, _stat.BaseMagicPoint);
        _baseValue.Add(DiamondType.Rage, _stat.BaseRagePoint);
        _baseValue.Add(DiamondType.Shield, _stat.BaseShieldPoint);
    }

    // private void InitShieldIconPosition()
    // {
    //     _shieldIcon.transform.localPosition = Vector3.one;
    // }
    // private void DisplayShieldIcon()
    // {
    //     if(_currentShieldPoint != 0) _shieldIcon.gameObject.SetActive(true);
    //     else _shieldIcon.gameObject.SetActive(false);
    // }

    public void AddHealthPoint(int healthToAdd)
    {
        _currentHealthPoint += healthToAdd;
        _currentHealthPoint = Mathf.Clamp(_currentHealthPoint, 0, _stat.MaxHealthPoint);
    }

    public void AddMagicPoint(int mpToAdd)
    {
        _currentMagicPoint += mpToAdd;
        _currentMagicPoint = Mathf.Clamp(_currentMagicPoint, 0, _stat.MaxManaPoint);
    }


    public void AddRagePoint(int rageToAdd)
    {
        _currentRagePoint += rageToAdd;
        _currentRagePoint = Mathf.Clamp(_currentRagePoint, 0, _stat.MaxRagePoint);
    }
    public void AddShieldPoint(int shieldToAdd)
    {
        _currentShieldPoint += shieldToAdd;
    }


    public IEnumerator ApplyEffect(DiamondType diamondType, int value)
    {
        if (_effectDictionary.ContainsKey(diamondType))
        {
            yield return _effectDictionary[diamondType].Execute(value);
            MessageManager.SendMessage(new Message(GameMessageType.OnApplyEffectStart, new object[] { diamondType, value }));
        }
    }

    public int CalculateDamage(int value)
    {
        int damage = value * _stat.BaseDamage;
        if (_currentRagePoint >= _stat.RagePointToIncreaseDamage)
        {
            _currentRagePoint -= _stat.RagePointToIncreaseDamage;
            _currentRagePoint = Mathf.Clamp(_currentRagePoint, 0, _stat.MaxRagePoint);
            damage = (int)(damage * _stat.DamageScale);
        }
        return damage;
    }

    public int CalculatePoint(DiamondType type, int diamondCount)
    {
        if (!_baseValue.ContainsKey(type)) return 0;
        return _baseValue[type] * diamondCount;

    }

    public void TakeDamage(int damage)
    {
        if (_currentShieldPoint != 0)
        {
            _currentShieldPoint = 0;
            damage = 0;
        }
        _currentHealthPoint -= damage;
        _currentHealthPoint = Mathf.Clamp(_currentHealthPoint, 0, _stat.MaxHealthPoint);
        if (damage != 0)
        {
            _animationHandler.SetHurtState();
        }
        MessageManager.SendMessage(new Message(GameMessageType.OnTakeDamage, new object[] { damage }));
    }

    public IEnumerator DeathHandle()
    {
        if (_currentHealthPoint <= 0)
        {
            yield return _animationHandler.SetDieState();
            Debug.Log("UNIT DIED");
            Side side = GetComponent<GameUnit>().UnitSide;
            MessageManager.SendMessage(new Message(GameMessageType.OnGameUnitDied, new object[] { side }));
        }
    }

    public bool IsDeath()
    {
        return _currentHealthPoint <= 0;
    }

    public void DealDamage(int damage)
    {
        BattleManager.Instance.EnemyUnit.StatHandler.TakeDamage(damage);
    }


    public void ApplyDiamondAttack()
    {
        int diamondCount = BattleManager.Instance.GetAttackMatchedCount();
        int damage = CalculateDamage(diamondCount);
        DealDamage(damage);
    }

    public void ResetShieldPoint()
    {
        _currentShieldPoint = 0;
    }

    public IEnumerator OnStolen(DiamondType diamondType, int value)
    {
        if (_effectDictionary.ContainsKey(diamondType))
        {
            yield return _effectDictionary[diamondType].Execute(this, -value);
            MessageManager.SendMessage(new Message(GameMessageType.OnValueStolen, new object[] { diamondType, -value }));
        }
    }


    public List<DiamondType> GetStealableTypes()
    {
        List<DiamondType> stealableTypes = new List<DiamondType>();
        if (_currentHealthPoint > 0)
        {
            stealableTypes.Add(DiamondType.Health);
        }
        if (_currentMagicPoint > 0)
        {
            stealableTypes.Add(DiamondType.MagicPoint);
        }
        if (_currentRagePoint > 0)
        {
            stealableTypes.Add(DiamondType.Rage);
        }
        return stealableTypes;
    }
}
