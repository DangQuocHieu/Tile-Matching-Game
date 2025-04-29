using System;
using UnityEngine;

public class BattleManager : Singleton<BattleManager>
{
    [SerializeField] private GameObject _leftUnit;
    [SerializeField] private GameObject _rightUnit;

    public GameObject GetEnemyUnit(Side side)
    {
        return side == Side.LeftSide ? _rightUnit : _leftUnit;
    }
}
