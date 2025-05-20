using UnityEngine.UI;
using UnityEngine;
using TMPro;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine.EventSystems;
public class GameCard : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private CardData _data;
    public CardData Data => _data;

    [SerializeField] private Image _cardImage;
    [SerializeField] private Button _cardButton;
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private RectTransform _descriptionPanel;
    [SerializeField] private TextMeshProUGUI _mpCostText;
    [SerializeField] private TextMeshProUGUI _rpCostText;

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
    }

    public void EnableCard()
    {
        _canvasGroup.alpha = 1;
        _canvasGroup.interactable = true;
    }
    public IEnumerator ApplyCardEffect()
    {
        MessageManager.SendMessage(new Message(GameMessageType.OnCardUsing, new object[] { _data.CardSprite }));
        PlayerCardController.Instance.DisableAllCard();
        TurnManager.Instance.PauseCurrentTurn();
        BattleManager.Instance.CurrentUnit.StatHandler.AddMagicPoint(-_data.MagicPointCost);
        BattleManager.Instance.CurrentUnit.StatHandler.AddRagePoint(-_data.RagePointCost);
        yield return _data.CardEffectSO.Activate();
        MessageManager.SendMessage(new Message(GameMessageType.OnApplyCardEffectEnd));
        gameObject.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_data.MagicPointCost == 0 && _data.RagePointCost == 0) return;
        _descriptionPanel.gameObject.SetActive(true);
        _mpCostText.text = "Magic Point: " + _data.MagicPointCost;
        _rpCostText.text = "Rage Point: " + _data.RagePointCost;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _descriptionPanel.gameObject.SetActive(false);
    }
}
