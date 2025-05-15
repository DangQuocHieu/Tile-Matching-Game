using UnityEngine.UI;
using UnityEngine;
using TMPro;
using System.Collections;
public class GameCard : MonoBehaviour
{
    [SerializeField] private CardData _data;
    public CardData Data => _data;

    [SerializeField] private Image _cardImage;
    [SerializeField] private Button _cardButton;
    [SerializeField] private CanvasGroup _canvasGroup;

    void Awake()
    {
        _cardButton = GetComponent<Button>();
        _canvasGroup = GetComponent<CanvasGroup>();
    }

    void Start()
    {
        AddButtonListener();
        DisableCard();
    }

    public void Init(CardData cardData)
    {
        _data = cardData;
        _cardImage.sprite = _data.CardSprite;
    }


    private void AddButtonListener()
    {
        _cardButton.onClick.AddListener(() =>
        {
            StartCoroutine(ApplyCardEffect());
            _canvasGroup.interactable = false;
            _canvasGroup.alpha = 0;
        });
    }

    public void DisableCard()
    {
        _canvasGroup.interactable = false;
        _canvasGroup.blocksRaycasts = false;
    }

    public void EnableCard()
    {
        _canvasGroup.alpha = 1;
        _canvasGroup.interactable = true;
        _canvasGroup.blocksRaycasts = true;
    }
    public IEnumerator ApplyCardEffect()
    {
        PlayerCardController.Instance.DisableAllCard();
        TurnManager.Instance.PauseCurrentTurn();
        BattleManager.Instance.CurrentUnit.StatHandler.AddMagicPoint(-_data.ManaPointToUse);
        yield return _data.CardEffectSO.Activate();
        _data.CardEffectSO.OnComplete(gameObject);
    }   


}
