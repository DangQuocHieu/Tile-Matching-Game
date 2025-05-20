using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerCardController : Singleton<PlayerCardController>, IMessageHandle
{
    [SerializeField] private GameCard _gameCardPrefab;
    private List<GameCard> _gameCards = new List<GameCard>();
    [SerializeField] private Transform _cardContainer;
    [SerializeField] private Side _side = Side.LeftSide;
    
    private int _currentManaPoint;
    private int _currentRagePoint;

    void Start()
    {
        InitCard();
    }
    void OnEnable()
    {
        MessageManager.AddSubscriber(GameMessageType.OnProcessBoardStart, this);
        MessageManager.AddSubscriber(GameMessageType.OnTurnStart, this);
        MessageManager.AddSubscriber(GameMessageType.OnCurrentTurnEnd, this);
    }

    void OnDisable()
    {
        MessageManager.RemoveSubscriber(GameMessageType.OnProcessBoardStart, this);
        MessageManager.RemoveSubscriber(GameMessageType.OnTurnStart, this);
        MessageManager.RemoveSubscriber(GameMessageType.OnCurrentTurnEnd, this);
    }

    private void InitCard()
    {
        List<CardData> selectedGameCards = CardSelectionController.Instance.GetSelectedGameCards();
        if (selectedGameCards != null)
        {
            foreach (var cardData in selectedGameCards)
            {
                GameCard card = Instantiate(_gameCardPrefab, _cardContainer);
                card.Init(cardData);
                _gameCards.Add(card);
            }
        }
    }

    public void Handle(Message message)
    {
        switch (message.type)
        {
            case GameMessageType.OnProcessBoardStart:
                DisableAllCard();
                break;
            case GameMessageType.OnApplyCardEffectEnd:
                EnableAllCard();
                break;
            case GameMessageType.OnTurnStart:
                {
                    if (_side == TurnManager.Instance.CurrentSide)
                        EnableAllCard();
                    break;
                }
            case GameMessageType.OnCurrentTurnEnd:
                {
                    if (_side == TurnManager.Instance.CurrentSide)
                        DisableAllCard();
                    break;
                }
        }
    }

    public void DisableAllCard()
    {
        foreach (var card in _gameCards)
        {
            if (card != null)
                card.DisableCard();
        }
    }

    private void EnableAllCard()
    {
        _currentManaPoint = transform.GetChild(0).GetComponent<UnitStatHandler>().CurrentMagicPoint;
        _currentRagePoint = transform.GetChild(0).GetComponent<UnitStatHandler>().CurrentRagePoint;
        foreach (var card in _gameCards)
        {
            if (card != null && _currentManaPoint >= card.Data.MagicPointCost && _currentRagePoint >= card.Data.RagePointCost) 
            {
                card.EnableCard();
            }
        }
    }
}
