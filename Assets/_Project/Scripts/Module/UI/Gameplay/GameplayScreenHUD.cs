using System;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GameplayScreenHUD : MonoBehaviour, IMessageHandle
{
    [Header("Text UI")]
    [SerializeField] private TextMeshProUGUI _turnTimerText;
    [SerializeField] private TextMeshProUGUI _playerNameText;
    [SerializeField] private TextMeshProUGUI _enemyNameText;
    [SerializeField] private TextMeshProUGUI _healthFloatingText;
    [SerializeField] private TextMeshProUGUI _magicPointFloatingText;
    [SerializeField] private TextMeshProUGUI _rageFloatingText;
    [SerializeField] private TextMeshProUGUI _shieldFloatingText;
    [SerializeField] RectTransform _turnDataContainer;
    [SerializeField] RectTransform _matchedDiamondUIPrefab;
    [SerializeField] Image _cardImage;

    [SerializeField] private Sprite[] _diamondSprites;
    [SerializeField] private DiamondType[] _diamondTypes;
    private Dictionary<DiamondType, Sprite> _spriteDictionary = new Dictionary<DiamondType, Sprite>();
    private Dictionary<DiamondType, TextMeshProUGUI> _floatingTextDictionary = new Dictionary<DiamondType, TextMeshProUGUI>();
    private int _currentIndex = 0;

    [Header("Stat Bar")]
    [SerializeField] private StatBarHUD _playerStatBarHUD;
    [SerializeField] private StatBarHUD _enemyStatBarHUD;

    [Header("Damage Box")]
    [SerializeField] private RectTransform _damageBox;
    [SerializeField] private TextMeshProUGUI _damageText;

    [Header("Animation Config")]
    [SerializeField] ScaleAmimationSO _scaleAnim;

    private GameUnit _leftUnit;
    private GameUnit _rightUnit;

    void Awake()
    {
        for (int i = 0; i < _diamondSprites.Length; i++)
        {
            _spriteDictionary.Add(_diamondTypes[i], _diamondSprites[i]);
        }

    }

    void OnEnable()
    {
        MessageManager.AddSubscriber(GameMessageType.OnCharacterLoaded, this);
        MessageManager.AddSubscriber(GameMessageType.OnTurnInProgress, this);
        MessageManager.AddSubscriber(GameMessageType.OnTurnStartDelay, this);
        MessageManager.AddSubscriber(GameMessageType.OnApplyEffectStart, this);
        MessageManager.AddSubscriber(GameMessageType.OnCombatPhaseStart, this);
        MessageManager.AddSubscriber(GameMessageType.OnTakeDamage, this);
        MessageManager.AddSubscriber(GameMessageType.OnApplyGainCard, this);
        MessageManager.AddSubscriber(GameMessageType.OnValueStolen, this);


    }

    void Start()
    {
        InitFloatingTextDictionary();
    }

    private void Update()
    {
        UpdateUnitStatBarHUD();
    }

    void OnDisable()
    {
        MessageManager.RemoveSubscriber(GameMessageType.OnCharacterLoaded, this);
        MessageManager.RemoveSubscriber(GameMessageType.OnTurnInProgress, this);
        MessageManager.RemoveSubscriber(GameMessageType.OnTurnStartDelay, this);
        MessageManager.RemoveSubscriber(GameMessageType.OnApplyEffectStart, this);
        MessageManager.RemoveSubscriber(GameMessageType.OnCombatPhaseStart, this);
        MessageManager.RemoveSubscriber(GameMessageType.OnTakeDamage, this);
        MessageManager.RemoveSubscriber(GameMessageType.OnApplyGainCard, this);
        MessageManager.RemoveSubscriber(GameMessageType.OnValueStolen, this);
    }

    public void Handle(Message message)
    {
        switch (message.type)
        {
            case GameMessageType.OnCharacterLoaded:
                _leftUnit = BattleManager.Instance.LeftUnit;
                _rightUnit = BattleManager.Instance.RightUnit;
                _playerStatBarHUD.Init(_leftUnit);
                _enemyStatBarHUD.Init(_rightUnit);
                break;
            case GameMessageType.OnTurnInProgress:
                float timeRemaining = (float)message.data[0];
                UpdateTimerText(timeRemaining);
                break;
            case GameMessageType.OnTurnStartDelay:
                float turnDuration = (float)message.data[0];
                UpdateTimerText(turnDuration);
                GetCurrentUnit();
                ResetMatchDiamondUI();
                ResetIndex();
                break;
            case GameMessageType.OnApplyEffectStart or GameMessageType.OnApplyGainCard:
                {
                    DiamondType diamondType = (DiamondType)message.data[0];
                    int value = (int)message.data[1];
                    ShowFloatingText(diamondType, value, showInCurrentUnitPosition: true);
                    AnimateMatchedDiamondUI();
                    break;
                }

            case GameMessageType.OnApplyCardEffectEnd:
                HideCardImage();
                break;
            case GameMessageType.OnCombatPhaseStart:
                SortedDictionary<DiamondType, int> dictionary = (SortedDictionary<DiamondType, int>)message.data[0];
                DisplayMatchStatUI(dictionary);
                break;
            case GameMessageType.OnTakeDamage:
                int damage = (int)message.data[0];
                ShowDamageBox(damage);
                break;
            case GameMessageType.OnValueStolen:
                {
                    DiamondType type = (DiamondType)message.data[0];
                    int value = (int)message.data[1];
                    ShowFloatingText(type, value, showInCurrentUnitPosition: false);
                    break;
                }
        }
    }

    private void InitFloatingTextDictionary()
    {
        _floatingTextDictionary.Add(DiamondType.Health, _healthFloatingText);
        _floatingTextDictionary.Add(DiamondType.MagicPoint, _magicPointFloatingText);
        _floatingTextDictionary.Add(DiamondType.Rage, _rageFloatingText);
        _floatingTextDictionary.Add(DiamondType.Shield, _shieldFloatingText);
    }

    private void AnimateMatchedDiamondUI()
    {
        if (_turnDataContainer.transform.childCount == 0) return;
        Transform matchDiamondUI = _turnDataContainer.transform.GetChild(_currentIndex);
        ++_currentIndex;
        _scaleAnim.ScaleOut(matchDiamondUI.gameObject);
    }

    private void ShowFloatingText(DiamondType type, int value, bool showInCurrentUnitPosition)
    {
        if (!_floatingTextDictionary.ContainsKey(type)) return;
        TextMeshProUGUI current = _floatingTextDictionary[type];
        current.gameObject.SetActive(true);
        if (value > 0) current.text = "+" + value;
        else current.text = value.ToString();
        if (showInCurrentUnitPosition)
        {
            current.transform.position = Camera.main.WorldToScreenPoint(BattleManager.Instance.CurrentUnit.transform.parent.position);
        }
        else
        {
            current.transform.position = Camera.main.WorldToScreenPoint(BattleManager.Instance.EnemyUnit.transform.parent.position);
        }

        _scaleAnim.ScaleIn(current.gameObject, () =>
        {
            current.gameObject.transform.localScale = Vector3.zero;
            current.gameObject.SetActive(false);
        });
    }

    private void ShowDamageBox(int damage)
    {
        _damageBox.gameObject.SetActive(true);
        _damageText.text = damage.ToString();
        _damageBox.transform.position = Camera.main.WorldToScreenPoint(BattleManager.Instance.EnemyUnit.transform.parent.position);
        DOVirtual.DelayedCall(0.5f, () =>
        {
            _damageBox.gameObject.SetActive(false);
        });
    }

    private void ShowCardImage(Sprite sprite)
    {
        _cardImage.gameObject.SetActive(true);
        _cardImage.sprite = sprite;
    }

    private void HideCardImage()
    {
        _cardImage.gameObject.SetActive(false);
    }

    private void UpdateTimerText(float time)
    {
        _turnTimerText.text = Mathf.FloorToInt(time).ToString();
    }

    private void DisplayMatchStatUI(SortedDictionary<DiamondType, int> matchedDiamonds)
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

    private void GetCurrentUnit()
    {
        _leftUnit = BattleManager.Instance.LeftUnit;
        _rightUnit = BattleManager.Instance.RightUnit;
    }
    private void UpdateUnitStatBarHUD()
    {

        _playerStatBarHUD.UpdateStatBar(_leftUnit);
        _playerStatBarHUD.UpdateTextUI(_leftUnit);
        _enemyStatBarHUD.UpdateStatBar(_rightUnit);
        _enemyStatBarHUD.UpdateTextUI(_rightUnit);
    }

}
