using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameEndScreen : UIScreen
{
    [SerializeField] private TextMeshProUGUI _titleText;
    [SerializeField] private Button _gobackButton;
    [SerializeField] private Button _menuButton;
    [SerializeField] private Button _restartButton;

    void Start()
    {
        AddButtonListener();
    }

    public void SetTitleText(string title)
    {
        _titleText.text = title.ToString();
    }
    public override void Hide()
    {
        gameObject.SetActive(false);
    }

    public override void Show()
    {
        gameObject.SetActive(true);
    }

    private void AddButtonListener()
    {
        _gobackButton.onClick.AddListener(() =>
        {
            ScreenManager.Instance.InitScreen();
            SceneManager.LoadSceneAsync("Character Selection Scene");
        });

        _menuButton.onClick.AddListener(() =>
        {
            ScreenManager.Instance.InitScreen();
            SceneManager.LoadSceneAsync("Menu Scene");
        });
        _restartButton.onClick.AddListener(() =>
        {
            MessageManager.SendMessage(new Message(GameMessageType.OnGameRestart));
            SceneManager.LoadSceneAsync("Gameplay Scene");
        });
    }
}
