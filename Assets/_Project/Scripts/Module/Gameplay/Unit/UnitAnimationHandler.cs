using System.Collections;
using Unity.VisualScripting;
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

    public IEnumerator SetMeleeAttackState(int index)
    {
        _unitAnimator.SetInteger(_meleeAttackString, index);
        yield return new WaitForNextFrameUnit();
        AnimatorStateInfo stateInfo = _unitAnimator.GetCurrentAnimatorStateInfo(0);
        yield return new WaitForSeconds(stateInfo.length);
        _unitAnimator.SetInteger(_meleeAttackString, -1);
    }

    public IEnumerator SetDieState()
    {
        yield return new WaitForEndOfFrame();
        _unitAnimator.SetTrigger(_dieString);
        while (!_unitAnimator.GetCurrentAnimatorStateInfo(0).IsName(_dieString.ToUpper()))
        {
            yield return null;
        }
        AnimatorStateInfo stateInfo = _unitAnimator.GetCurrentAnimatorStateInfo(0);
        yield return new WaitForSeconds(stateInfo.length);
    }
}
