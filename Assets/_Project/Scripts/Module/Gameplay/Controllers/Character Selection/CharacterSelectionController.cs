using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectionController : Singleton<CharacterSelectionController>, IMessageHandle
{
    [SerializeField] private GridLayoutGroup _gridGroup;
    [SerializeField] List<CharacterSelectionButton> _characterButton;
    [SerializeField] RectTransform _playerSelector;
    [SerializeField] RectTransform _enemySelector;

    [SerializeField] private int _playerIndex = 0;
    [SerializeField] private int _enemyIndex = 1;
    private int _gridColumn;

    [SerializeField] private CharacterCard _playerCard;
    [SerializeField] private CharacterCard _enemyCard;

    public void Init()
    {
        _gridColumn = _gridGroup.constraintCount;
        if(_gridGroup != null)
        _characterButton = _gridGroup.GetComponentsInChildren<CharacterSelectionButton>().ToList();
        SetUpSelector();
    }   

    void OnEnable()
    {
        MessageManager.AddSubscriber(GameMessageType.OnCharacterSelectionButtonLoaded, this);
    }

    void OnDisable()
    {
        MessageManager.AddSubscriber(GameMessageType.OnCharacterSelectionButtonLoaded, this);
    }

    public void SetUpSelector()
    {
        if (_playerSelector == null || _enemySelector == null) return;
        UpdateSelectorPosition();
        _playerSelector.gameObject.SetActive(true);
        _enemySelector.gameObject.SetActive(true);
    }

    private void Update()
    {
        UpdateSelectorPosition();
        HandlePlayerSelectorInput();
        HandleEnemySelectorInput();
        UpdateCardVisual();
    }


    public void UpdateSelectorPosition()
    {
        if(_characterButton.Count == 0) return;
        if (_playerSelector == null) return;
        if (_enemySelector == null) return;
        _playerSelector.position = _characterButton[_playerIndex].transform.position;
        _enemySelector.position = _characterButton[_enemyIndex].transform.position;
    }

    private void MoveSelector(ref int index, int delta)
    {
        int newIndex = index + delta;
        if (newIndex >= 0 && newIndex < _characterButton.Count)
        {
            index = newIndex;
        }
    }

    private void HandlePlayerSelectorInput()
    {
        if (Input.GetKeyDown(KeyCode.W)) MoveSelector(ref _playerIndex, -_gridColumn);
        if (Input.GetKeyDown(KeyCode.S)) MoveSelector(ref _playerIndex, _gridColumn);
        if (Input.GetKeyDown(KeyCode.A)) MoveSelector(ref _playerIndex, -1);
        if (Input.GetKeyDown(KeyCode.D)) MoveSelector(ref _playerIndex, 1);
    }

    private void HandleEnemySelectorInput()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow)) MoveSelector(ref _enemyIndex, -_gridColumn);
        if (Input.GetKeyDown(KeyCode.DownArrow)) MoveSelector(ref _enemyIndex, _gridColumn);
        if (Input.GetKeyDown(KeyCode.LeftArrow)) MoveSelector(ref _enemyIndex, -1);
        if (Input.GetKeyDown(KeyCode.RightArrow)) MoveSelector(ref _enemyIndex, 1);

    }

    private void UpdateCardVisual()
    {
        UpdatePlayerCard();
        UpdateEnemyCard();
    }

    private void UpdatePlayerCard()
    {
        if(_characterButton.Count == 0) return;
        _playerCard.UpdateCardValue(_characterButton[_playerIndex].Config);
    }

    private void UpdateEnemyCard()
    {
        if(_characterButton.Count == 0) return;
        _enemyCard.UpdateCardValue(_characterButton[_enemyIndex].Config);
    }

    public void SaveCharacter()
    {
        PlayerPrefs.SetString("Player Character", _characterButton[_playerIndex].Config.Stat.UnitId);
        PlayerPrefs.SetString("Enemy Character", _characterButton[_enemyIndex].Config.Stat.UnitId);
    }

    public void Handle(Message message)
    {
        switch(message.type)
        {
            case GameMessageType.OnCharacterSelectionButtonLoaded:
                Init();
                Debug.Log("INIT");
                break;
        }
    }
}
