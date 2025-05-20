    using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "MeleeAttack", menuName = "Scriptable Objects/AttackStrategy/MeleeAttack")]
public class MeleeAttack : AttackStrategySO
{
    [SerializeField] private float _dashTime = 1;
    [SerializeField] private float _goBackDelay = 0.5f;
    [SerializeField] private float _attackDelay = 1f;
    [SerializeField] private int _attackCount;
    [SerializeField] private float[] _xOffset;
    private Dictionary<int, float> _xOffsetDictionary = new Dictionary<int, float>();

    void OnEnable()
    {
        _xOffsetDictionary.Clear();
        for(int i = 0; i < _attackCount; i++)
        {
            _xOffsetDictionary.Add(i, _xOffset[i]);
        }
    }
    public override IEnumerator Execute(GameUnit attacker, GameUnit target)
    {
        yield return new WaitForSeconds(_attackDelay);
        int attackIndex = Random.Range(0, _attackCount);
        float xAttackPos = _xOffsetDictionary[attackIndex];
        xAttackPos = attacker.UnitSide == Side.LeftSide ? xAttackPos : -xAttackPos;
        attacker.AnimationHandler.SetRunState();
        yield return attacker.transform.DOMoveX(attacker.transform.position.x + xAttackPos, _dashTime).SetEase(Ease.Linear).WaitForCompletion();
        yield return attacker.AnimationHandler.SetMeleeAttackState(attackIndex);
        yield return new WaitForSeconds(_goBackDelay);
        yield return Rotate(attacker.transform);
        attacker.AnimationHandler.SetRunState();
        yield return attacker.transform.DOMoveX(attacker.transform.position.x - xAttackPos, _dashTime).SetEase(Ease.Linear).WaitForCompletion();
        yield return Rotate(attacker.transform);
        attacker.AnimationHandler.SetIdleState();


    }

    private IEnumerator Rotate(Transform attacker)
    {
        yield return new WaitForEndOfFrame();
        attacker.localScale = new Vector3(-attacker.transform.localScale.x, attacker.transform.localScale.y, attacker.transform.localScale.z);
    }
}
