using UnityEngine;
public enum CardType
{
    GainCard, AttackCard, ClearCard
}

[CreateAssetMenu(fileName = "CardData", menuName = "Scriptable Objects/CardData")]
public class CardData : ScriptableObject
{
    [SerializeField] private CardType _cardType;
    public CardType CardType => _cardType;
    [SerializeField] private int _mpPointToUse;
    public int MpPointToUse => _mpPointToUse;
    [SerializeField] private int _ragePointToUse;
    public int RagePointToUse => _ragePointToUse;
    [SerializeField] private Sprite _cardSprite;
    public Sprite CardSprite => _cardSprite;
    [SerializeField] CardEffectSO _cardEffectSO;
    public CardEffectSO CardEffectSO => _cardEffectSO;
    [SerializeField] private string _description;
    public string Description => _description;

    public bool CanUse(int currentMagicPoint, int currentRagePoint)
    {
        return currentMagicPoint >= _mpPointToUse && currentRagePoint >= _ragePointToUse;
    }
}
