using System.Collections;
using System.Collections.Generic;
using NUnit.Framework.Constraints;
using UnityEngine;

public class EffectHandler : MonoBehaviour, IMessageHandle
{
    private AttackHandler _attackHandler;
    [SerializeField] private float _applyEffectDelay = 1f;
    private SortedDictionary<DiamondType, int> _matchedDiamondDictionary = new SortedDictionary<DiamondType, int>();
    private Dictionary<DiamondType, IEffect> _effectDictionary = new Dictionary<DiamondType, IEffect>();

    private UnitStatHandler _statHanlder;
    void Awake()
    {
        InitializeDictionary();
        _statHanlder = GetComponent<UnitStatHandler>();
        _attackHandler = GetComponent<AttackHandler>();
    }
    void OnEnable()
    {
        MessageManager.AddSubcriber(GameMessageType.OnDiamondDestroy, this);
        MessageManager.AddSubcriber(GameMessageType.OnTurnStartDelay, this);
        MessageManager.AddSubcriber(GameMessageType.OnBoardProcessed, this);
    }

    void OnDisable()
    {
        MessageManager.RemoveSubcriber(GameMessageType.OnDiamondDestroy, this);
        MessageManager.RemoveSubcriber(GameMessageType.OnTurnStartDelay, this);
        MessageManager.RemoveSubcriber(GameMessageType.OnBoardProcessed, this);
    }

    public void Handle(Message message)
    {
        if (!GetComponent<GameUnit>().IsInTurn()) return;
        switch (message.type)
        {
            case GameMessageType.OnDiamondDestroy:
                DiamondType type = (DiamondType)message.data[0];
                UpdateMatchedDiamondDictionary(type);
                break;
            case GameMessageType.OnTurnStartDelay:
                ResetMatchedDiamondDictionary();
                break;
            case GameMessageType.OnBoardProcessed:
                StartCoroutine(OnBoardProcessed());
                break;
        }
    }
    void InitializeDictionary()
    {
        _effectDictionary.Add(DiamondType.Health, new HealthEffect());
        _effectDictionary.Add(DiamondType.Mana, new ManaEffect());
        _effectDictionary.Add(DiamondType.Rage, new RageEffect());
        _effectDictionary.Add(DiamondType.Shield, new ShieldEffect());
    }
    private void UpdateMatchedDiamondDictionary(DiamondType diamondType)
    {
        if (!_matchedDiamondDictionary.ContainsKey(diamondType))
        {
            _matchedDiamondDictionary[diamondType] = 1;
        }
        else ++_matchedDiamondDictionary[diamondType];
    }

    private void ResetMatchedDiamondDictionary()
    {
        _matchedDiamondDictionary.Clear();
        _matchedDiamondDictionary = new SortedDictionary<DiamondType, int>();
    }

    private IEnumerator OnBoardProcessed()
    {
        //Update UI 
        MessageManager.SendMessage(new Message(GameMessageType.OnMatchStatUIUpdated, new object[] { _matchedDiamondDictionary }));
        //Apply Effect
        yield return new WaitForSeconds(_applyEffectDelay);
        yield return ApplyEffect();
        yield return PerformAttack();
        MessageManager.SendMessage(new Message(GameMessageType.OnCurrentTurnEnd));

    }

    private IEnumerator ApplyEffect()
    {
        foreach (var item in _matchedDiamondDictionary)
        {
            if (_effectDictionary.ContainsKey(item.Key))
            {
                MessageManager.SendMessage(new Message(GameMessageType.OnApplyEffect, new object[] {item.Key}));
                yield return new WaitForSeconds(1f);
                _effectDictionary[item.Key].ApplyEffect(_statHanlder, item.Value);
            }
        }
    }

    private IEnumerator PerformAttack()
    {
        if(_matchedDiamondDictionary.ContainsKey(DiamondType.Attack))
        {
            MessageManager.SendMessage(new Message(GameMessageType.OnApplyEffect, new object[] {DiamondType.Attack}));
            yield return new WaitForSeconds(1f);

            int damage = _statHanlder.AttackDamage * _matchedDiamondDictionary[DiamondType.Attack];
            yield return _attackHandler.Attack(damage);
        }
    }

}
