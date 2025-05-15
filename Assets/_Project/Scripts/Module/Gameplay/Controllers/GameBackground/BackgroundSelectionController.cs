using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class BackgroundSelectionController : PersistentSingleton<BackgroundSelectionController>
{
    [SerializeField] private Sprite[] _bgSprites;
    [SerializeField] private BackgroundSelector _bgSelectorPrefab;
    [SerializeField] private RectTransform _bgSelectorContainer;
    [SerializeField] private RectTransform _comfirmIcon;

    private BackgroundSelector _currentSelector;
    public BackgroundSelector CurrentSelector => _currentSelector;

    protected override void Awake()
    {
        base.Awake();
        Init();
    }
    private void Init()
    {
        for (int i = 0; i < _bgSprites.Count(); i++)
        {
            BackgroundSelector selector = Instantiate(_bgSelectorPrefab, _bgSelectorContainer);
            selector.Init(_bgSprites[i]);
            if(i == 0) _currentSelector = selector;
        }
    }

    public void HandleBackgroundSelection(BackgroundSelector selector)
    {
        _comfirmIcon.gameObject.SetActive(true);
        _comfirmIcon.transform.position = selector.transform.position;
        _currentSelector = selector;
        
    }
}
