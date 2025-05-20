using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ScreenManager : PersistentSingleton<ScreenManager>, IMessageHandle
{
    private Stack<ScreenKey> _stackScreen = new Stack<ScreenKey>();
    private Dictionary<ScreenKey, UIScreen> _screenDictionary = new Dictionary<ScreenKey, UIScreen>();

    protected override void Awake()
    {
        base.Awake();
        InitScreenDictionary();
        InitScreen();
    }

    private void OnEnable()
    {
        MessageManager.AddSubscriber(GameMessageType.OnGameWin, this);
        MessageManager.AddSubscriber(GameMessageType.OnGameLose, this);
        MessageManager.AddSubscriber(GameMessageType.OnGameRestart, this);
        MessageManager.AddSubscriber(GameMessageType.OnGamePaused, this);
    }

    private void OnDisable()
    {
        MessageManager.RemoveSubscriber(GameMessageType.OnGameWin, this);
        MessageManager.RemoveSubscriber(GameMessageType.OnGameLose, this);
        MessageManager.RemoveSubscriber(GameMessageType.OnGameRestart, this);
        MessageManager.RemoveSubscriber(GameMessageType.OnGamePaused, this);
    }

    private void InitScreenDictionary()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            UIScreen screen = transform.GetChild(i).GetComponent<UIScreen>();
            _screenDictionary.Add(screen.Key, screen);
        }
    }

    public void InitScreen()
    {
        foreach (var screenKey in _screenDictionary.Keys)
        {
            _screenDictionary[screenKey].gameObject.SetActive(false);
        }
        _stackScreen.Clear();
        _stackScreen = new Stack<ScreenKey>();
    }
    public void ShowScreen(ScreenKey _keyToShow)
    {
        if (_stackScreen.Count != 0)
        {
            ScreenKey currentScreenKey = _stackScreen.Peek();
            _screenDictionary[currentScreenKey].Hide();
        }
        _stackScreen.Push(_keyToShow);
        _screenDictionary[_keyToShow].Show();
    }

    public void GoBack()
    {
        ScreenKey currentScreen = _stackScreen.Peek();
        _screenDictionary[currentScreen].Hide();
        _stackScreen.Pop();
        if (_stackScreen.Count != 0)
        {
            _screenDictionary[_stackScreen.Peek()].Show();
        }
    }

    public void Handle(Message message)
    {
        switch (message.type)
        {
            case GameMessageType.OnGameWin:
                ShowScreen(ScreenKey.GameEnd);
                _screenDictionary[ScreenKey.GameEnd].GetComponent<GameEndScreen>().SetTitleText("YOU WIN");
                break;

            case GameMessageType.OnGameLose:
                ShowScreen(ScreenKey.GameEnd);
                _screenDictionary[ScreenKey.GameEnd].GetComponent<GameEndScreen>().SetTitleText("YOU LOSE");
                break;

            case GameMessageType.OnGameRestart:
                InitScreen();
                break;

            case GameMessageType.OnGamePaused:
                ShowScreen(ScreenKey.Pause);
                break;
        }
    }
}
