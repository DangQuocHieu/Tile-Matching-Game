using System.Collections;
using UnityEngine;

public class UnitAnimationHandler : MonoBehaviour
{
    private Animator _unitAnimator;
    [Header("String for animation trigger")]
    private string _idleString = "Idle";
    private string _runString = "Run";
    private string _meleeAttackString = "meleeAttackIndex";
    private string _hurtString = "Hurt";
    private string _dieString = "Die";    
    void Awake()
    {
        _unitAnimator = GetComponent<Animator>();
    }

    public void SetIdleState()
    {
        _unitAnimator.SetTrigger(_idleString);
    }

    public void SetRunState()
    {
        _unitAnimator.SetTrigger(_runString);
    }

    public void SetHurtState()
    {
        _unitAnimator.SetTrigger(_hurtString);
    }

    public IEnumerator SetMeleeAttackState(float damageTriggerOffset, int index)
    {
        yield return new WaitForEndOfFrame();
        _unitAnimator.SetInteger(_meleeAttackString, index);
        AnimatorStateInfo stateInfo = _unitAnimator.GetCurrentAnimatorStateInfo(0);
        yield return new WaitForSeconds(stateInfo.length - damageTriggerOffset);
        _unitAnimator.SetInteger(_meleeAttackString, -1);
    }

    public void SetDieState()
    {
        _unitAnimator.SetTrigger(_dieString);
    }
}
