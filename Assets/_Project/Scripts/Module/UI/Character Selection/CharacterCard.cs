using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
public class CharacterCard : MonoBehaviour
{
    [SerializeField] private Image _characterImage;
    [Header("TEXT UI")]
    [SerializeField] TextMeshProUGUI _healthPointText;
    [SerializeField] TextMeshProUGUI _manaPointText;
    [SerializeField] TextMeshProUGUI _ragePointText;
    [SerializeField] TextMeshProUGUI _baseDamageText;
    [SerializeField] TextMeshProUGUI _healthPerDiamondText;
    [SerializeField] TextMeshProUGUI _manaPerDiamondText;
    [SerializeField] TextMeshProUGUI _ragePerDiamondText;
    [SerializeField] TextMeshProUGUI _stealPointText;

    public void UpdateCardValue(CharacterSelectionConfig config)
    {
        if(config == null) return;
        _characterImage.sprite = config.CharacterImage;
        UpdateText(config);
    
    }

    private void UpdateText(CharacterSelectionConfig config)
    {
        _healthPointText.text = config.Stat.MaxHealthPoint.ToString();
        _manaPerDiamondText.text = config.Stat.MaxManaPoint.ToString();
        _ragePointText.text = config.Stat.MaxRagePoint.ToString();
        _baseDamageText.text = config.Stat.BaseDamage.ToString();
        _healthPerDiamondText.text = config.Stat.BaseHealthPoint.ToString();
        _manaPerDiamondText.text = config.Stat.BaseManaPoint.ToString();
        _ragePerDiamondText.text = config.Stat.BaseRagePoint.ToString();
    }
}
