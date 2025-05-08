using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : Singleton<BattleManager>, IMessageHandle
{
    [SerializeField] private GameUnit _leftUnit;
    [SerializeField] private GameUnit _rightUnit;

    public GameUnit LeftUnit => _leftUnit;
    public GameUnit RightUnit => _rightUnit;

    [SerializeField] private GameUnit _currentUnit;
    public GameUnit CurrentUnit => _currentUnit;
    [SerializeField] private GameUnit _enemyUnit;
    public GameUnit EnemyUnit => _enemyUnit;
    
    [SerializeField] private string _attackSortingLayerName = "Attack";
    [SerializeField] private string _unitSortingLayerName = "Unit";
    private SortedDictionary<DiamondType, int> _matchedDiamondDictionary = new SortedDictionary<DiamondType, int>();
    void Start()
    {
        InitSide();
    }

    void OnEnable()
    {
        MessageManager.AddSubcriber(GameMessageType.OnTurnStartDelay, this);
        MessageManager.AddSubcriber(GameMessageType.OnDiamondDestroy, this);
        MessageManager.AddSubcriber(GameMessageType.OnBoardProcessed, this);

    }

    void OnDisable()
    {
        MessageManager.RemoveSubcriber(GameMessageType.OnTurnStartDelay, this);
        MessageManager.RemoveSubcriber(GameMessageType.OnDiamondDestroy, this);
        MessageManager.RemoveSubcriber(GameMessageType.OnBoardProcessed, this);
    }



    private void InitSide()
    {
        _leftUnit.InitSide(Side.LeftSide);
        _rightUnit.InitSide(Side.RightSide);
    }

    public void Handle(Message message)
    {
        switch (message.type)
        {
            case GameMessageType.OnTurnStartDelay:
                ChangeCurrentUnit();
                ResetMatchedDiamondDictionary();
                break;
            case GameMessageType.OnDiamondDestroy:
                DiamondType type = (DiamondType)message.data[0];
                UpdateMatchedDiamondDictionary(type);
                break;
            case GameMessageType.OnBoardProcessed:
                StartCoroutine(CombatPhase());
                break;
        }
    }

    private void ChangeCurrentUnit()
    {
        GameUnit unit = TurnManager.Instance.CurrentSide == Side.LeftSide ? _leftUnit : _rightUnit;
        _currentUnit = unit;
        _enemyUnit = _currentUnit == _leftUnit ? _rightUnit : _leftUnit;
        _currentUnit.GetComponent<SpriteRenderer>().sortingLayerName = _attackSortingLayerName;
        _enemyUnit.GetComponent<SpriteRenderer>().sortingLayerName = _unitSortingLayerName;
    }
    private void ResetMatchedDiamondDictionary()
    {
        _matchedDiamondDictionary.Clear();
        _matchedDiamondDictionary = new SortedDictionary<DiamondType, int>();
    }

    private void UpdateMatchedDiamondDictionary(DiamondType type)
    {
        if (!_matchedDiamondDictionary.ContainsKey(type))
        {
            _matchedDiamondDictionary[type] = 1;
        }
        else _matchedDiamondDictionary[type]++;
    }

    private IEnumerator CombatPhase()
    {
        MessageManager.SendMessage(new Message(GameMessageType.OnCombatPhaseStart, new object[] { _matchedDiamondDictionary }));
        yield return new WaitForSeconds(1f);
        yield return ApplyEffectPhase();
        yield return AttackPhase();
        MessageManager.SendMessage(new Message(GameMessageType.OnCurrentTurnEnd));
    }

    private IEnumerator ApplyEffectPhase()
    {
        foreach (var item in _matchedDiamondDictionary)
        {
            yield return _currentUnit.StatHandler.ApplyEffect(item.Key, item.Value);
        }
    }

    private IEnumerator AttackPhase()
    {
        if (_matchedDiamondDictionary.ContainsKey(DiamondType.Attack))
        {
          
            yield return _currentUnit.AttackHandler.Attack(_currentUnit, _enemyUnit, () =>
            {
                int damage = _currentUnit.StatHandler.CalculateDamage(_matchedDiamondDictionary[DiamondType.Attack]);
                _enemyUnit.StatHandler.TakeDamage(damage);
            });
        }
    }
}
