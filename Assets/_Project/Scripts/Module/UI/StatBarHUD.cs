using UnityEngine;
using UnityEngine.UI;

public class StatBarHUD : MonoBehaviour
{
    [SerializeField] private Slider _healthSlider;
    [SerializeField] private Slider _manaSlider;
    [SerializeField] private Slider _rageSlider;

    public void Init(int maxHealthPoint, int maxManaPoint, int maxRagePoint)
    {
        _healthSlider.maxValue = maxHealthPoint;
        _manaSlider.maxValue = maxManaPoint;
        _rageSlider.maxValue = maxRagePoint;
    }

    public void UpdateStatBar(int currentHealth, int currentMana, int currentRage)
    {
        _healthSlider.value = currentHealth;
        _manaSlider.value = currentMana;
        _rageSlider.value = currentRage;
    } 
}
