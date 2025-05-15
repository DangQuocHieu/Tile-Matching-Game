using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CardSelectionController : PersistentSingleton<CardSelectionController>
{
    [SerializeField] private List<CardData> _allCards = new List<CardData>();
    [SerializeField] private List<CardData> _availableCards = new List<CardData>();
    [SerializeField] private List<CardData> _selectedCards = new List<CardData>();
    [SerializeField] private CardSelector _cardSelectorPrefab;
    [SerializeField] private Transform _avaiableCardContainter;
    [SerializeField] private Transform _selectedCardContainer;
    [SerializeField] private int _capacity = 4;


    void Start()
    {
        InitAvailableCard();
    }
    private void InitAvailableCard()
    {

        for (int i = 0; i < _availableCards.Count; i++)
        {
            CardSelector card = Instantiate(_cardSelectorPrefab, _avaiableCardContainter);
            card.Init(_availableCards[i]);
        }
        _allCards = new List<CardData>(_availableCards);
    }

    public void HandleCardSelection(CardSelector card)
    {
        if (card.transform.parent == _avaiableCardContainter)
        {
            SelectCard(card);
        }
        else
        {
            DeselectCard(card);
        }
    }

    public void SelectCard(CardSelector card)
    {
        if (_selectedCards.Count >= _capacity) return;
        _availableCards.Remove(card.Data);
        _selectedCards.Add(card.Data);
        card.transform.SetParent(_selectedCardContainer);
        card.DisableDescription();
    }

    public void DeselectCard(CardSelector card)
    {
        card.EnableDescription();
        _availableCards.Add(card.Data);
        _selectedCards.Remove(card.Data);
        card.transform.SetParent(_avaiableCardContainter);
        SortAvailableCards();
    }

    public List<CardData> GetSelectedGameCards()
    {
        return _selectedCards;
    }

    private void SortAvailableCards()
    {
        var children = _avaiableCardContainter.Cast<Transform>().ToList();
        children.Sort((a, b) =>
        {
            var aData = a.GetComponent<CardSelector>().Data;
            var bData = b.GetComponent<CardSelector>().Data;
            return _allCards.IndexOf(aData).CompareTo(_allCards.IndexOf(bData));
        });

        for (int i = 0; i < children.Count; i++)
        {
            children[i].SetSiblingIndex(i);
        }
    }



}
