using UnityEngine;
using DG.Tweening;
using System.Collections;
[CreateAssetMenu(fileName = "ProjectileRangedAttack", menuName = "Scriptable Objects/Card Effect/ProjectileRangedAttack")]
public class ProjectileRangedAttackEffect : CardEffectSO
{
    [SerializeField] private int _effectValue;
    [SerializeField] private GameObject _projectilePrefab;
    [SerializeField] private float _yOffset = 2f;
    public override IEnumerator Activate()
    {
        BoardManager.Instance.HideDiamondBoard();
        GameUnit currentUnit = BattleManager.Instance.CurrentUnit;
        GameUnit enemyUnit = BattleManager.Instance.EnemyUnit;
        Vector3 spawnPos = currentUnit.transform.position + new Vector3(0, _yOffset, 0);
        GameObject projectileGO = Instantiate(_projectilePrefab, spawnPos, Quaternion.identity);
        projectileGO.transform.DOMoveX(enemyUnit.transform.position.x, _applyDuration).SetEase(Ease.Linear);
        yield return new WaitForSeconds(_applyDuration);
        enemyUnit.StatHandler.TakeDamage(_effectValue);
        Destroy(projectileGO);
        BoardManager.Instance.ShowDiamondBoard();
    }

}
