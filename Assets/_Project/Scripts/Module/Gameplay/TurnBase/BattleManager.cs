using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BattleManager : Singleton<BattleManager>, IMessageHandle
{
    [SerializeField] private List<GameUnit> _gameUnitPrefabs = new List<GameUnit>();
    [SerializeField] private PlayerController _playerController;
    [SerializeField] private EnemyController _enemyController;
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

    [SerializeField] private bool _loadCharacter = true;
    
    private void Start()
    {
        if(_loadCharacter) LoadCharacter();
        else InitSide();
    }

    void OnEnable()
    {
        MessageManager.AddSubscriber(GameMessageType.OnTurnStartDelay, this);
        MessageManager.AddSubscriber(GameMessageType.OnDiamondDestroy, this);
        MessageManager.AddSubscriber(GameMessageType.OnBoardProcessed, this);

    }

    void OnDisable()
    {
        MessageManager.RemoveSubscriber(GameMessageType.OnTurnStartDelay, this);
        MessageManager.RemoveSubscriber(GameMessageType.OnDiamondDestroy, this);
        MessageManager.RemoveSubscriber(GameMessageType.OnBoardProcessed, this);
    }

    private void LoadCharacter()
    {
        string playerCharacterId = PlayerPrefs.GetString("Player Character");
        string enemyCharacterId = PlayerPrefs.GetString("Enemy Character");
        Debug.Log(playerCharacterId);
        Debug.Log(enemyCharacterId);
        GameUnit playerUnit = _gameUnitPrefabs.Where(T => T.GetComponent<UnitStatHandler>().Stat.UnitId == playerCharacterId).FirstOrDefault();
        GameUnit enemyUnit = _gameUnitPrefabs.Where(T => T.GetComponent<UnitStatHandler>().Stat.UnitId == enemyCharacterId).FirstOrDefault();
        _leftUnit = Instantiate(playerUnit, _playerController.transform);
        _rightUnit = Instantiate(enemyUnit, _enemyController.transform);
        InitSide(); 
    }

    private void InitSide()
    {
        _leftUnit.InitSide(Side.LeftSide);
        _rightUnit.InitSide(Side.RightSide);
        _currentUnit = _leftUnit;
        _enemyUnit = _rightUnit;
        MessageManager.SendMessage(new Message(GameMessageType.OnCharacterLoaded));
    }

    public void Handle(Message message)
    {
        switch (message.type)
        {
            case GameMessageType.OnTurnStartDelay:
                Side side = (Side)message.data[2];
                ChangeCurrentUnit(side);
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

    private void ChangeCurrentUnit(Side side)
    {
        
        GameUnit unit = side == Side.LeftSide ? _leftUnit : _rightUnit;
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
            int pointToAdd = _currentUnit.StatHandler.CalculatePoint(item.Key, item.Value);
            yield return _currentUnit.StatHandler.ApplyEffect(item.Key, pointToAdd);
        }
    }

    private IEnumerator AttackPhase()
    {
        if(_matchedDiamondDictionary.ContainsKey(DiamondType.Steal))
        {
            yield return _currentUnit.AttackHandler.Steal(_currentUnit, _enemyUnit);
        }
        if (_matchedDiamondDictionary.ContainsKey(DiamondType.Attack))
        {
            yield return _currentUnit.AttackHandler.Attack(_currentUnit, _enemyUnit);
        }
    }

    public int GetAttackMatchedCount()
    {
        return _matchedDiamondDictionary[DiamondType.Attack];
    }

    public int GetStealMatchedCount()
    {
        return _matchedDiamondDictionary[DiamondType.Steal];
    }

    
}
