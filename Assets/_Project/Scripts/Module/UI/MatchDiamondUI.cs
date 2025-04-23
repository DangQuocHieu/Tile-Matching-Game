using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MatchDiamondUI : MonoBehaviour
{
    [SerializeField] private Image _diamondImage;
    [SerializeField] private TextMeshProUGUI _counterText;


    public void Init(Sprite sprite, int counter)
    {
        _diamondImage.sprite = sprite;
        _counterText.text = counter.ToString();
    }
}
