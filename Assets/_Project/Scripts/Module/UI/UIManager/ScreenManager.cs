using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ScreenManager : PersistentSingleton<ScreenManager>
{
    private Stack<ScreenKey> _stackScreen = new Stack<ScreenKey>();
    private Dictionary<ScreenKey, UIScreen> _screenDictionary = new Dictionary<ScreenKey, UIScreen>();
    [SerializeField] private ScreenKey _initialScreenKey;

    protected override void Awake()
    {
        base.Awake();
        InitScreenDictionary();
        InitScreen();
    }
    private void InitScreenDictionary()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            UIScreen screen = transform.GetChild(i).GetComponent<UIScreen>();
            _screenDictionary.Add(screen.Key, screen);
        }
    }

    private void InitScreen()
    {
        foreach (var screenKey in _screenDictionary.Keys)
        {
            _screenDictionary[screenKey].gameObject.SetActive(false);
        }
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
        if(_stackScreen.Count != 0)
        {
            _screenDictionary[_stackScreen.Peek()].Show();
        }
    }


}
