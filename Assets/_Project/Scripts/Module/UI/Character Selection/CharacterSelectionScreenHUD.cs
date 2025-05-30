using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CharacterSelectionScreenHUD : MonoBehaviour
{
    [SerializeField] private RectTransform _characterSelectionBar;
    [SerializeField] private CharacterSelectionConfig[] _configs;
    [SerializeField] private CharacterSelectionButton _selectButtonPrefab;
    [SerializeField] private Button _cardSelectButton;
    [SerializeField] private Button _playButton;
    [SerializeField] private Button _backgroundSelectionButton;
    private CanvasGroup _canvasGroup;
    
    void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
    }

    void Start()
    {
        AddButtonListener();
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
        MessageManager.SendMessage(new Message(GameMessageType.OnCharacterSelectionButtonLoaded));
    }

    private void AddButtonListener()
    {
        
        _cardSelectButton.onClick.AddListener(()=>{
            ScreenManager.Instance.ShowScreen(ScreenKey.CardSelect);
        });

        _playButton.onClick.AddListener(()=>{
            CharacterSelectionController.Instance.SaveCharacter();
            SceneManager.LoadSceneAsync("Gameplay Scene");
        });
        _backgroundSelectionButton.onClick.AddListener(()=>{
            ScreenManager.Instance.ShowScreen(ScreenKey.BackgroundSelect);
        });
    }
}
