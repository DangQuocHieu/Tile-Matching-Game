using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public class CharacterSelectionScreenHUD : MonoBehaviour
{
    [SerializeField] private RectTransform _characterSelectionBar;
    [SerializeField] private CharacterSelectionConfig[] _configs;
    [SerializeField] private CharacterSelectionButton _selectButtonPrefab;
    
    void Awake()
    {
        Init();
    }

    private void Init()
    {
        for(int i = 0; i < _characterSelectionBar.childCount; i++)
        {
            Destroy(_characterSelectionBar.GetChild(i));
        }
        foreach(var config in _configs)
        {
            var button = Instantiate(_selectButtonPrefab, _characterSelectionBar);
            button.Init(config);            
        }
    }


}
