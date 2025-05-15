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
        _healthSlider.maxValue = gameUnit.GetComponent<UnitStatHandler>().Stat.MaxHealthPoint;
        _manaSlider.maxValue = gameUnit.GetComponent<UnitStatHandler>().Stat.MaxManaPoint;
        _rageSlider.maxValue = gameUnit.GetComponent<UnitStatHandler>().Stat.MaxRagePoint;
    }

    public void UpdateStatBar(GameUnit gameUnit)
    {
        if(gameUnit.StatHandler == null) return;
        _healthSlider.value = gameUnit.StatHandler.CurrentHealthPoint;
        _manaSlider.value = gameUnit.StatHandler.CurrentMagicPoint;
        _rageSlider.value = gameUnit.StatHandler.CurrentRagePoint;
    } 

    public void UpdateTextUI(GameUnit gameUnit)
    {
        if(gameUnit.StatHandler == null) return;
        _healthText.text = gameUnit.StatHandler.CurrentHealthPoint + "/" + gameUnit.StatHandler.Stat.MaxHealthPoint;
        _manaText.text = gameUnit.StatHandler.CurrentMagicPoint + "/" + gameUnit.StatHandler.Stat.MaxManaPoint;
        _rageText.text = gameUnit.StatHandler.CurrentRagePoint + "/" + gameUnit.StatHandler.Stat.MaxRagePoint;
    }
}
