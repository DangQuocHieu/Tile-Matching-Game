using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectionController : MonoBehaviour
{
    [SerializeField] private GridLayoutGroup _gridGroup;
    [SerializeField] List<CharacterSelectionButton> _characterButton;
    [SerializeField] RectTransform _playerSelector;
    [SerializeField] RectTransform _enemySelector;

    [SerializeField] private int _playerIndex = 0;
    [SerializeField] private int _enemyIndex = 1;
    private int _gridColumn;

    [SerializeField] CharacterCard _playerCard;
    [SerializeField] CharacterCard _enemyCard;
    private void Start()
    {
        Init();
        SetUpSelector();
    }

    private void Init()
    {
        _gridColumn = _gridGroup.constraintCount;
        _characterButton = _gridGroup.GetComponentsInChildren<CharacterSelectionButton>().ToList();
    }

    private void SetUpSelector()
    {
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


    private void UpdateSelectorPosition()
    {
        _playerSelector.position = _characterButton[_playerIndex].transform.position;
        _enemySelector.position = _characterButton[_enemyIndex].transform.position;
    }

    private void MoveSelector(ref int index, int delta)
    {
        int newIndex = index + delta;
        if(newIndex >= 0 && newIndex < _characterButton.Count)
        {
            index = newIndex;
        }
    }

    private void HandlePlayerSelectorInput()
    {
        if(Input.GetKeyDown(KeyCode.W)) MoveSelector(ref _playerIndex, -_gridColumn);
        if(Input.GetKeyDown(KeyCode.S)) MoveSelector(ref _playerIndex, _gridColumn);
        if(Input.GetKeyDown(KeyCode.A)) MoveSelector(ref _playerIndex, -1);
        if(Input.GetKeyDown(KeyCode.D)) MoveSelector(ref _playerIndex, 1);
    }

    private void HandleEnemySelectorInput()
    {
        if(Input.GetKeyDown(KeyCode.UpArrow)) MoveSelector(ref _enemyIndex, -_gridColumn);
        if(Input.GetKeyDown(KeyCode.DownArrow)) MoveSelector(ref _enemyIndex, _gridColumn);
        if(Input.GetKeyDown(KeyCode.LeftArrow)) MoveSelector(ref _enemyIndex, -1);
        if(Input.GetKeyDown(KeyCode.RightArrow)) MoveSelector(ref _enemyIndex, 1);

    }

    private void UpdateCardVisual()
    {
        UpdatePlayerCard();
        UpdateEnemyCard();
    }

    private void UpdatePlayerCard()
    {
        _playerCard.UpdateCardValue(_characterButton[_playerIndex].Config);
    }

    private void UpdateEnemyCard()
    {
        _enemyCard.UpdateCardValue(_characterButton[_enemyIndex].Config);
    }
}
