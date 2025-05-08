using System.Collections;
using System.Collections.Generic;
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
    [SerializeField] private int _currentManaPoint;
    public int CurrentManaPoint => _currentManaPoint;
    [SerializeField] private int _currentRagePoint;
    public int CurrentRagePoint => _currentRagePoint;
    [SerializeField] private int _currentShieldPoint;
    public int CurrentShieldPoint => _currentShieldPoint;

    private Dictionary<DiamondType, IEffect> _effectDictionary = new Dictionary<DiamondType, IEffect>();
    [SerializeField] private float _applyEffectDuration = 1f;
    // [SerializeField] private GameObject _shieldIcon;

    private UnitAnimationHandler _animationHandler;

    void Start()
    {
        _animationHandler = GetComponent<UnitAnimationHandler>();
        InitializeStat();
        InitializeEffectDictionary();
        // InitShieldIconPosition();
    }

    void Update()
    {
        // DisplayShieldIcon();
    }
    private void InitializeStat()
    {
        _currentHealthPoint = _stat.MaxHealthPoint;
        _currentManaPoint = 0;
        _currentRagePoint = 0;
    }

    private void InitializeEffectDictionary()
    {
        _effectDictionary.Add(DiamondType.Health, new HealthEffect());
        _effectDictionary.Add(DiamondType.Mana, new ManaEffect());
        _effectDictionary.Add(DiamondType.Rage, new RageEffect());
        _effectDictionary.Add(DiamondType.Shield, new ShieldEffect());
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

    public IEnumerator AddHealthPoint(int value)
    {
        yield return new WaitForSeconds(_applyEffectDuration);
        int healthToAdd = value * _stat.BaseHealthPoint;
        _currentHealthPoint += healthToAdd;
        _currentHealthPoint = Mathf.Clamp(_currentHealthPoint,0, _stat.MaxHealthPoint);
        MessageManager.SendMessage(new Message(GameMessageType.OnApplyEffectStart, new object[] {DiamondType.Health, healthToAdd}));
        
    }

    public IEnumerator AddManaPoint(int value)
    {
        yield return new WaitForSeconds(_applyEffectDuration);
        int manaToAdd = value * _stat.BaseManaPoint;
        _currentManaPoint += manaToAdd;
        _currentManaPoint = Mathf.Clamp(_currentManaPoint,0, _stat.MaxManaPoint);
        MessageManager.SendMessage(new Message(GameMessageType.OnApplyEffectStart, new object[] {DiamondType.Mana, manaToAdd}));
    }

    public IEnumerator AddRagePoint(int value)
    {
        yield return new WaitForSeconds(_applyEffectDuration);
        int rageToAdd = value * _stat.BaseRagePoint;
        _currentRagePoint += rageToAdd;
        _currentRagePoint = Mathf.Clamp(_currentRagePoint, 0, _stat.MaxRagePoint);      
        MessageManager.SendMessage(new Message(GameMessageType.OnApplyEffectStart, new object[] {DiamondType.Rage, rageToAdd}));  
    }

    public IEnumerator AddShieldPoint(int value)
    {
        yield return new WaitForSeconds(_applyEffectDuration);
        int shieldToAdd = value * _stat.BaseShieldPoint;
        _currentShieldPoint += shieldToAdd; 
        MessageManager.SendMessage(new Message(GameMessageType.OnApplyEffectStart, new object[] {DiamondType.Shield, shieldToAdd}));
    }

    public IEnumerator ApplyEffect(DiamondType diamondType, int value)
    {
        if(_effectDictionary.ContainsKey(diamondType))
        {
            yield return _effectDictionary[diamondType].ApplyEffect(this, value);
        }
    }

    public int CalculateDamage(int value)
    {
        int damage = value * _stat.BaseDamage;
        if(_currentRagePoint == _stat.RagePointToIncreaseDamage)
        {
            _currentRagePoint -= _stat.RagePointToIncreaseDamage;
            _currentRagePoint = Mathf.Clamp(_currentRagePoint, 0, _stat.MaxRagePoint);
            damage = (int)(damage * _stat.DamageScale);
        }
        return damage;
    }

    public void TakeDamage(int damage)
    {
        if(_currentShieldPoint != 0)
        {
            _currentShieldPoint = 0;
            damage = 0;
        }
        _currentHealthPoint -= damage;
        _currentHealthPoint = Mathf.Clamp(_currentHealthPoint,0, _stat.MaxHealthPoint);
        if(damage != 0)
        {
            _animationHandler.SetHurtState();
        }
        MessageManager.SendMessage(new Message(GameMessageType.OnTakeDamage, new object[] {damage}));
        if(damage == 0)
        {
            //DIE ?
        }
    }
}
