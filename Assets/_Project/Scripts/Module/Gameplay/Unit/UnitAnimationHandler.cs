using UnityEngine;

public class UnitAnimationHandler : MonoBehaviour, IMessageHandle
{

    private Side _side;
    [SerializeField] private Animator _unitAnim;
    [Header("String for animation trigger")]
    [SerializeField] private string _idleString = "Idle";
    [SerializeField] private string _runString = "Run";
    [SerializeField] private string _meleeAttackString = "MeleeAttack";
    [SerializeField] private string _rangedAttackString = "RangedAttack";
    [SerializeField] private string _hurtAttackString = "Hurt";


    void Start()
    {
        _side = GetComponent<GameUnit>().Side;
    }

    void OnEnable()
    {
        MessageManager.AddSubcriber(GameMessageType.OnAttack, this);
    }

    void OnDisable()
    {
        MessageManager.AddSubcriber(GameMessageType.OnAttack, this);
    }

    public void SetIdleAnimation()
    {
        if(_unitAnim == null) return;
        _unitAnim.SetTrigger(_idleString);
    }

    public void SetRunAnimation()
    {
        if(_unitAnim == null) return;
        _unitAnim.SetTrigger(_runString);
    }

    public void SetMeleeAttackAnimation()
    {
        if(_unitAnim == null) return;
        _unitAnim.SetTrigger(_meleeAttackString);
    }

    public void SetRangedAttackAnimation()
    {
        if(_unitAnim == null) return;
        _unitAnim.SetTrigger(_rangedAttackString);
    }

    public void SetHurAnimation()
    {
        if(_unitAnim == null) return;
        _unitAnim.SetTrigger(_hurtAttackString);
    }

    public void Handle(Message message)
    {
        switch(message.type)
        {
            case GameMessageType.OnAttack:
                if(_side != TurnManager.Instance.CurrentSide)
                {
                    SetHurAnimation();
                }
                break;
        }
    }
}
