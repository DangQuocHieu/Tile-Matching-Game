using System.Collections;
using DG.Tweening;
using UnityEngine;

[CreateAssetMenu(fileName = "MeleeAttack", menuName = "Scriptable Objects/AttackStrategy/MeleeAttack")]
public class MeleeAttack : AttackStrategySO
{
    [SerializeField] private float _dashTime = 1f;
    [SerializeField] private float _attackDuration;
    [SerializeField] private float _goBackDelay = 1f;
    [SerializeField] private float _xOffset = 2f; 
    
    public override IEnumerator Execute(GameObject attacker, GameObject target, TweenCallback callback = null)
    {
        UnitAnimationHandler handler = attacker.GetComponent<UnitAnimationHandler>();
        Vector3 initialPosition = attacker.transform.position;
        Side side = attacker.gameObject.GetComponent<GameUnit>().Side;
        Vector3 offset = (side == Side.LeftSide) ? new Vector3(-_xOffset, 0, 0) : new Vector3(+_xOffset, 0, 0);

        handler.SetRunAnimation();
        yield return attacker.transform.DOMove(target.transform.position + offset, _dashTime).SetEase(Ease.Linear).WaitForCompletion();
        handler.SetMeleeAttackAnimation();
        yield return new WaitForSeconds(_attackDuration);
        callback?.Invoke(); 
        yield return new WaitForSeconds(_goBackDelay);
        handler.SetRunAnimation();
        yield return attacker.transform.DOMove(initialPosition,_dashTime).SetEase(Ease.Linear).WaitForCompletion();
        handler.SetIdleAnimation();
    }
}
