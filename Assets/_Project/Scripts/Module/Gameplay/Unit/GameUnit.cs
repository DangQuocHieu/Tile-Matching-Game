using UnityEngine;
[RequireComponent(typeof(UnitStatHandler), typeof(UnitAnimationHandler))]
[RequireComponent(typeof(UnitAttackHandler), typeof(Animator))]
public class GameUnit : MonoBehaviour
{
    private UnitStatHandler _statHandler;
    public UnitStatHandler StatHandler => _statHandler;
    private UnitAnimationHandler _animationHandler;
    public UnitAnimationHandler AnimationHandler => _animationHandler;
    private UnitAttackHandler _attackHandler;
    public UnitAttackHandler AttackHandler => _attackHandler;
    [SerializeField] private Side _unitSide;
    public Side UnitSide => _unitSide;
    void Start()
    {
        _statHandler = GetComponent<UnitStatHandler>();
        _animationHandler = GetComponent<UnitAnimationHandler>();
        _attackHandler = GetComponent<UnitAttackHandler>();
    }

    public void InitSide(Side side)
    {
        _unitSide = side;
    }  
    

    
}
