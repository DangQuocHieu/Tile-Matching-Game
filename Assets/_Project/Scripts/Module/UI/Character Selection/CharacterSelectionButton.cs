using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectionButton : MonoBehaviour
{
    [SerializeField] private CharacterSelectionConfig _config;
    public CharacterSelectionConfig Config => _config;

    [SerializeField] private Image _characterImage;
    private string _id;
    public string ID => _id;
    public void Init(CharacterSelectionConfig config)
    {
        _config = config;
        _characterImage.sprite = config.CharacterIcon;
        _id = config.Stat.UnitId;

    }
}
