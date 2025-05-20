using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseScreen : UIScreen
{
    [SerializeField] private Button _gobackButton;
    [SerializeField] private Button _menuButton;
    [SerializeField] private Button _continueButton;

    void Start()
    {
        AddButtonListener();
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
        _continueButton.onClick.AddListener(() =>
        {
            MessageManager.SendMessage(new Message(GameMessageType.OnGameContinued));
            ScreenManager.Instance.InitScreen();
        });
    }


}
