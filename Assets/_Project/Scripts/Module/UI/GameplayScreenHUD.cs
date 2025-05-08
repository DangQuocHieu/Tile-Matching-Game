using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class GameplayScreenHUD : MonoBehaviour, IMessageHandle
{
    [Header("Text UI")]
    [SerializeField] private TextMeshProUGUI _turnTimerText;
    [SerializeField] private TextMeshProUGUI _playerNameText;
    [SerializeField] private TextMeshProUGUI _enemyNameText;
    [SerializeField] private TextMeshProUGUI _healthFloatingText;
    [SerializeField] private TextMeshProUGUI _manaFloatingText;
    [SerializeField] private TextMeshProUGUI _rageFloatingText;
    [SerializeField] private TextMeshProUGUI _shieldFloatingText;
    [SerializeField] RectTransform _turnDataContainer;
    [SerializeField] RectTransform _matchedDiamondUIPrefab;

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

    private GameUnit _currentLeftUnit;
    private GameUnit _currentRightUnit;

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
        MessageManager.AddSubcriber(GameMessageType.OnApplyEffectStart, this);
        MessageManager.AddSubcriber(GameMessageType.OnCombatPhaseStart, this);
        MessageManager.AddSubcriber(GameMessageType.OnTakeDamage, this);
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
        MessageManager.RemoveSubcriber(GameMessageType.OnTurnInProgress, this);
        MessageManager.RemoveSubcriber(GameMessageType.OnTurnStartDelay, this);
        MessageManager.RemoveSubcriber(GameMessageType.OnApplyEffectStart, this);
        MessageManager.RemoveSubcriber(GameMessageType.OnCombatPhaseStart, this);
        MessageManager.RemoveSubcriber(GameMessageType.OnTakeDamage, this);
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
                GetCurrentUnit();
                ResetMatchDiamondUI();
                ResetIndex();
                ResetStatBarHUD();
                break;
            case GameMessageType.OnApplyEffectStart:
                DiamondType type = (DiamondType)message.data[0];
                int value = (int)message.data[1];
                ShowFloatingText(type, value);
                AnimateMatchedDiamondUI();
                break;
            case GameMessageType.OnCombatPhaseStart:
                SortedDictionary<DiamondType, int> dictionary = (SortedDictionary<DiamondType, int>)message.data[0];
                DisplayMatchStatUI(dictionary);
                break;
            case GameMessageType.OnTakeDamage:
                int damage = (int)message.data[0];
                ShowDamageBox(damage);
                break;
            
        }
    }

    private void InitFloatingTextDictionary()
    {
        _floatingTextDictionary.Add(DiamondType.Health, _healthFloatingText);
        _floatingTextDictionary.Add(DiamondType.Mana, _manaFloatingText);
        _floatingTextDictionary.Add(DiamondType.Rage, _rageFloatingText);
        _floatingTextDictionary.Add(DiamondType.Shield, _shieldFloatingText);
    }

    private void AnimateMatchedDiamondUI()
    {
        Transform matchDiamondUI = _turnDataContainer.transform.GetChild(_currentIndex);
        ++_currentIndex;
        _scaleAnim.ScaleOut(matchDiamondUI.gameObject);
    }

    private void ShowFloatingText(DiamondType type, int value)
    {
        if(!_floatingTextDictionary.ContainsKey(type)) return;
        TextMeshProUGUI current = _floatingTextDictionary[type];
        current.gameObject.SetActive(true);
        current.text = "+" + value;
        current.transform.position = Camera.main.WorldToScreenPoint(BattleManager.Instance.CurrentUnit.transform.parent.position);
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
        DOVirtual.DelayedCall(0.5f, ()=>{
            _damageBox.gameObject.SetActive(false);
        });
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
        _currentLeftUnit = BattleManager.Instance.LeftUnit;
        _currentRightUnit = BattleManager.Instance.RightUnit;
    }
    private void UpdateUnitStatBarHUD()
    {

        _playerStatBarHUD.UpdateStatBar(_currentLeftUnit);
        _playerStatBarHUD.UpdateTextUI(_currentLeftUnit);
        _enemyStatBarHUD.UpdateStatBar(_currentRightUnit);
        _enemyStatBarHUD.UpdateTextUI(_currentRightUnit);
    }

    private void ResetStatBarHUD()
    {
        _playerStatBarHUD.Init(_currentLeftUnit);
        _enemyStatBarHUD.Init(_currentRightUnit);
    }
}
