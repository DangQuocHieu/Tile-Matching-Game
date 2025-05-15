using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardSelector : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private CardData _data;
    public CardData Data => _data;
    [SerializeField] private Image _cardImage;
    [SerializeField] private Button _cardButton;
    [SerializeField] private RectTransform _descriptionPanel;
    [SerializeField] private TextMeshProUGUI _descriptionText;
    public bool _disableDescription = false;


    void Start()
    {
        AddButtonListener();
    }
    public void Init(CardData data)
    {
        _data = data;
        _cardImage.sprite = data.CardSprite;
        _descriptionText.text = _data.Description;
    }

    private void AddButtonListener()
    {
        _cardButton.onClick.AddListener(()=>{
            CardSelectionController.Instance.HandleCardSelection(this);
        });
    }

    public void DisableDescription()
    {
        _disableDescription = true;
    }

    public void EnableDescription()
    {
        _disableDescription = false;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _descriptionPanel.gameObject.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(_disableDescription) return;
        _descriptionPanel.gameObject.SetActive(true);
    }
}
