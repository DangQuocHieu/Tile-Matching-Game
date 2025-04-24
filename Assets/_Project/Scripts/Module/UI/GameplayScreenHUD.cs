using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class GameplayScreenHUD : MonoBehaviour, IMessageHandle
{
    [Header("Text UI")]
    [SerializeField] private TextMeshProUGUI _turnTimerText;
    [SerializeField] private TextMeshProUGUI _playerNameText;
    [SerializeField] private TextMeshProUGUI _enemyNameText;

    [SerializeField] RectTransform _turnDataContainer;
    [SerializeField] RectTransform _matchedDiamondUIPrefab;

    [SerializeField] private Sprite[] _diamondSprites;
    [SerializeField] private DiamondType[] _diamondTypes;
    private Dictionary<DiamondType, Sprite> _spriteDictionary = new Dictionary<DiamondType, Sprite>();
    private int _currentIndex = 0;

    [Header("Stat Bar")]
    [SerializeField] private StatBarHUD _playerStatBarHUD;
    [SerializeField] private StatBarHUD _enemyStatBarHUD;

    [Header("Animation Config")]
    [SerializeField] MoveToAnimationSO _moveToAnimationSO;

    private PlayerStat _playerStat;
    private EnemyStat _enemyStat;
    void Awake()
    {
        for (int i = 0; i < _diamondSprites.Length; i++)
        {
            _spriteDictionary.Add(_diamondTypes[i], _diamondSprites[i]);
        }

    }

    void OnEnable()
    {
        MessageManager.AddSubcriber(GameMessageType.OnTurnInProgress, this);
        MessageManager.AddSubcriber(GameMessageType.OnTurnStartDelay, this);
        MessageManager.AddSubcriber(GameMessageType.OnMatchResolve, this);
        MessageManager.AddSubcriber(GameMessageType.OnEnemyApplyEffect, this);
        MessageManager.AddSubcriber(GameMessageType.OnPlayerApplyEffect, this);
    }

    void Start()
    {
        _playerStat = PlayerController.Instance.GetComponent<PlayerStat>();
        _enemyStat = EnemyController.Instance.GetComponent<EnemyStat>();
        InitStatHUD();
    }

    private void Update()
    {
        UpdatePlayerStatHUD();
        UpdateEnemyStatHUD();
    }

    void OnDisable()
    {
        MessageManager.RemoveSubcriber(GameMessageType.OnTurnInProgress, this);
        MessageManager.RemoveSubcriber(GameMessageType.OnTurnStartDelay, this);
        MessageManager.RemoveSubcriber(GameMessageType.OnMatchResolve, this);
        MessageManager.RemoveSubcriber(GameMessageType.OnEnemyApplyEffect, this);
        MessageManager.RemoveSubcriber(GameMessageType.OnPlayerApplyEffect, this);
    }

    public void Handle(Message message)
    {
        switch (message.type)
        {
            case GameMessageType.OnTurnInProgress:
                float timeRemaining = (float)message.data[0];
                UpdateTimerText(timeRemaining);
                break;
            case GameMessageType.OnTurnStartDelay:
                float turnDuration = (float)message.data[0];
                UpdateTimerText(turnDuration);
                ResetMatchDiamondUI();
                ResetIndex();
                break;
            case GameMessageType.OnMatchResolve:
                Dictionary<DiamondType, int> matchDiamondCount = (Dictionary<DiamondType, int>)message.data[0];
                DisplayMatchDiamondUI(matchDiamondCount);
                break;
            case GameMessageType.OnEnemyApplyEffect:
                ApplyEffectUI(EnemyController.Instance.transform);
                break;
            case GameMessageType.OnPlayerApplyEffect:
                ApplyEffectUI(PlayerController.Instance.transform);
                break;
        }
    }

    private void InitStatHUD()
    {
        _playerStatBarHUD.Init(_playerStat.Stat.MaxHealthPoint, _playerStat.Stat.MaxManaPoint, _playerStat.Stat.MaxRagePoint);
        _enemyStatBarHUD.Init(_enemyStat.Stat.MaxHealthPoint, _enemyStat.Stat.MaxManaPoint, _enemyStat.Stat.MaxRagePoint);
    }

    private void ApplyEffectUI(Transform target)
    {
        Debug.Log("APPLY UI");
        Transform matchDiamondUI = _turnDataContainer.transform.GetChild(_currentIndex);
        ++_currentIndex;
        Vector3 position = Camera.main.WorldToScreenPoint(target.transform.position);
        _moveToAnimationSO.MoveTo(matchDiamondUI, position, () =>
        {

        });
    }


    private void UpdateTimerText(float time)
    {
        _turnTimerText.text = Mathf.FloorToInt(time).ToString();
    }

    private void DisplayMatchDiamondUI(Dictionary<DiamondType, int> matchedDiamonds)
    {
        foreach (var item in matchedDiamonds)
        {
            MatchDiamondUI ui = Instantiate(_matchedDiamondUIPrefab, _turnDataContainer).GetComponent<MatchDiamondUI>();
            ui.Init(_spriteDictionary[item.Key], item.Value);
        }
    }

    private void ResetMatchDiamondUI()
    {
        foreach (RectTransform child in _turnDataContainer.transform)
        {
            Destroy(child.gameObject);
        }
    }

    private void ResetIndex()
    {
        _currentIndex = 0;
    }

    private void UpdatePlayerStatHUD()
    {
        if (_playerStat != null)
        {
            _playerStatBarHUD.UpdateStatBar(_playerStat.CurrentHealthPoint, _playerStat.CurrentManaPoint, _playerStat.CurrentRagePoint);
        }

    }

    private void UpdateEnemyStatHUD()
    {
        if (_enemyStat != null)
        {
            _enemyStatBarHUD.UpdateStatBar(_enemyStat.CurrentHealthPoint, _enemyStat.CurrentManaPoint, _enemyStat.CurrentRagePoint);
        }
    }
}
