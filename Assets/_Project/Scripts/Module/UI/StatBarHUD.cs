using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatBarHUD : MonoBehaviour
{
    [Header("Slider")]
    [SerializeField] private Slider _healthSlider;
    [SerializeField] private Slider _manaSlider;
    [SerializeField] private Slider _rageSlider;

    [Header("Text")]
    [SerializeField] private TextMeshProUGUI _healthText;
    [SerializeField] private TextMeshProUGUI _manaText;
    [SerializeField] private TextMeshProUGUI _rageText;

    public void Init(GameUnit gameUnit)
    {
        _healthSlider.maxValue = gameUnit.StatHandler.Stat.MaxHealthPoint;
        _manaSlider.maxValue = gameUnit.StatHandler.Stat.MaxManaPoint;
        _rageSlider.maxValue = gameUnit.StatHandler.Stat.MaxRagePoint;
    }

    public void UpdateStatBar(GameUnit gameUnit)
    {
        _healthSlider.value = gameUnit.StatHandler.CurrentHealthPoint;
        _manaSlider.value = gameUnit.StatHandler.CurrentManaPoint;
        _rageSlider.value = gameUnit.StatHandler.CurrentRagePoint;
    } 

    public void UpdateTextUI(GameUnit gameUnit)
    {
        _healthText.text = gameUnit.StatHandler.CurrentHealthPoint + "/" + gameUnit.StatHandler.Stat.MaxHealthPoint;
        _manaText.text = gameUnit.StatHandler.CurrentManaPoint + "/" + gameUnit.StatHandler.Stat.MaxManaPoint;
        _rageText.text = gameUnit.StatHandler.CurrentRagePoint + "/" + gameUnit.StatHandler.Stat.MaxRagePoint;
    }
}
