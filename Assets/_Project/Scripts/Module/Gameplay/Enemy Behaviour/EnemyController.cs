using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class EnemyController : Singleton<EnemyController>, IMessageHandle
{
    [SerializeField] private float minDelay = 2f;
    [SerializeField] private float maxDelay = 5f;

    [SerializeField] private Side _enemySide = Side.RightSide;
    public Side EnemySide => _enemySide;
    [SerializeField] private List<CardData> _allGameCards;
    [SerializeField] private List<CardData> _enemyCards;
    [SerializeField] private AIStrategySO[] _strategies;
    [SerializeField] private int _cardCapacity = 5;
    private List<CardData> _usableCards = new List<CardData>();
    void Start()
    {
        InitializeEnemyCards();
    }
    private void OnEnable()
    {
        MessageManager.AddSubscriber(GameMessageType.OnTurnStart, this);
    }

    private void OnDisable()
    {
        MessageManager.RemoveSubscriber(GameMessageType.OnTurnStart, this);
    }

    private void PerformAction()
    {
        StartCoroutine(PerformActionCoroutine());
    }

    private IEnumerator PerformActionCoroutine()
    {
        yield return HandleCardUsing();
        yield return new WaitForSeconds(1f);
        List<Tuple<GameObject, GameObject>> validMoves = BoardManager.Instance.GenerateValidMoves();
        Tuple<GameObject, GameObject> bestMove = _strategies[0].FindBestMove(validMoves);
        DiamondController.Instance.SwapDiamond(bestMove.Item1, bestMove.Item2);
    }

    private void InitializeEnemyCards()
    {
        for (int i = 0; i < _cardCapacity; i++)
        {
            CardData selectedCard = _allGameCards[UnityEngine.Random.Range(0, _allGameCards.Count)];
            _enemyCards.Add(selectedCard);
            _allGameCards.Remove(selectedCard);
        }


    }
    public void Handle(Message message)
    {
        switch (message.type)
        {
            case GameMessageType.OnTurnStart:
                Side currentSide = (Side)message.data[0];
                if (_enemySide == currentSide)
                {
                    Invoke("PerformAction", UnityEngine.Random.Range(minDelay, maxDelay + 1));
                }
                break;
        }
    }

    public void UpdatetUsableCards()
    {
        _usableCards.Clear();
        _usableCards = new List<CardData>();
        int magicPoint = BattleManager.Instance.CurrentUnit.StatHandler.CurrentMagicPoint;
        int ragePoint = BattleManager.Instance.CurrentUnit.StatHandler.CurrentRagePoint;
        foreach (var cardData in _enemyCards)
        {
            if (cardData.CanUse(magicPoint, ragePoint))
            {
                _usableCards.Add(cardData);
            }
        }
    }

    public int GetCardRequiringMagicPoint()
    {
        int counter = 0;
        foreach (var cardData in _enemyCards)
        {
            if (cardData.MpPointToUse != 0) ++counter;
        }
        return counter;
    }

    private IEnumerator HandleCardUsing()
    {
        UpdatetUsableCards();
        if (_usableCards.Count == 0)
        {
            yield break;
        }
        CardData cardData = _strategies[0].FindBestCard(_usableCards);
        _enemyCards.Remove(cardData);
        yield return cardData.CardEffectSO.Activate();
        BattleManager.Instance.CurrentUnit.StatHandler.AddMagicPoint(cardData.MpPointToUse);
        BattleManager.Instance.CurrentUnit.StatHandler.AddRagePoint(cardData.RagePointToUse);
    }
}
