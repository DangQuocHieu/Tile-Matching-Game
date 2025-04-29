using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackHandler : MonoBehaviour
{
    private Side _side;
    [SerializeField] AttackStrategySO[] _attackStrategy;

    void Start()
    {
        _side = GetComponent<GameUnit>().Side;
    }
     
    public IEnumerator Attack(int damage)
    {
        GameObject target = BattleManager.Instance.GetEnemyUnit(_side);
        yield return _attackStrategy[Random.Range(0, _attackStrategy.Length)].Execute(this.gameObject, target, ()=> {
            MessageManager.SendMessage(new Message(GameMessageType.OnAttack, new object[] {damage, _side}));
        });
    }
}
