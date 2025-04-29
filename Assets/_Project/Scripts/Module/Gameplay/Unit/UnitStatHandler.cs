using UnityEngine;

public class UnitStatHandler : MonoBehaviour, IMessageHandle
{

    private Side _side;
    [SerializeField] protected UnitStat _stat;
    public UnitStat Stat => _stat;
    [SerializeField] protected int _currentHealthPoint;
    public int CurrentHealthPoint => _currentHealthPoint;
    [SerializeField] protected int _currentManaPoint;
    public int CurrentManaPoint => _currentManaPoint;
    [SerializeField] protected int _currentRagePoint;
    public int CurrentRagePoint => _currentRagePoint;
    [SerializeField] protected int _currentShieldPoint;
    [SerializeField] protected int _attackDamage;
    public int AttackDamage => _attackDamage;


    protected void Awake()
    {
        SetUpStat();
        _side = GetComponent<GameUnit>().Side;
    }

    void OnEnable()
    {
        MessageManager.AddSubcriber(GameMessageType.OnAttack, this);
    }

    void OnDisable()
    {
        MessageManager.RemoveSubcriber(GameMessageType.OnAttack, this);
    }

    private void SetUpStat()
    {
        _currentHealthPoint = _stat.MaxHealthPoint;
        _currentManaPoint = 0;
        _currentRagePoint = 0;
        _currentShieldPoint = 0;
        _attackDamage = _stat.BaseDamage;
    }


    public void AddManaPoint(int counter)
    {
        _currentManaPoint += counter * _stat.ManaIncrease;
        _currentManaPoint = Mathf.Clamp(_currentManaPoint, 0, _stat.MaxManaPoint);
    }

    public void AddRagePoint(int counter)
    {
        _currentRagePoint += counter * _stat.RageIncrease;
        _currentRagePoint = Mathf.Clamp(_currentRagePoint, 0, _stat.MaxRagePoint);
    }

    public void AddHealthPoint(int counter)
    {
        _currentHealthPoint += counter * _stat.HealthIncrease;
        _currentHealthPoint = Mathf.Clamp(_currentHealthPoint, 0, _stat.MaxHealthPoint);
    }

    public void AddShieldPoint(int counter)
    {
        _currentShieldPoint += counter * _stat.ShieldIncrease;
        _currentShieldPoint = Mathf.Clamp(_currentShieldPoint, 0, _stat.MaxShieldPoint);
    }
    
    public void TakeDamage(int damage)
    {
        int damageToShield = Mathf.Min(damage, _currentShieldPoint);
        AddShieldPoint(-damageToShield);
        damage -= damageToShield;
        _currentHealthPoint -= damage;
        _currentHealthPoint = Mathf.Clamp(_currentHealthPoint, 0, _stat.MaxHealthPoint);
        if(_currentHealthPoint <= 0)
        {
            //DIE
        }
    }

    public void Handle(Message message)
    {
        switch(message.type)
        {
            case GameMessageType.OnAttack:
                int damage = (int)message.data[0];
                Side side = (Side)message.data[1];
                if(side != _side)
                {
                    TakeDamage(damage);
                }
                break;

            
        }
    }
}
