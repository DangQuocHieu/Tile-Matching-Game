using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "InstantRangedAttack", menuName = "Scriptable Objects/Card Effect/InstantRangedAttack")]
public class InstantRangedAttackEffect : CardEffectSO
{
    [SerializeField] protected int _effectValue;
    [SerializeField] private GameObject _prefab;
    [SerializeField] private float yOffset;
    public override IEnumerator Activate()
    {
        BoardManager.Instance.HideDiamondBoard();
        GameUnit enemyUnit = BattleManager.Instance.EnemyUnit;
        Vector3 spawnPos = enemyUnit.transform.parent.position + new Vector3(0, yOffset, 0);
        GameObject prefabGO = Instantiate(_prefab, spawnPos, Quaternion.identity);
        yield return new WaitForSeconds(_applyDuration);
        enemyUnit.StatHandler.TakeDamage(_effectValue);
        Destroy(prefabGO);
    }
    public override void OnComplete(GameObject gameObject)
    {
        MessageManager.SendMessage(new Message(GameMessageType.OnApplyCardEffectEnd));
        Destroy(gameObject);
    }
}
