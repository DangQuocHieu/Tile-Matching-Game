using UnityEngine;
public enum CardType
{
    GainHPCard, GaimMagicPointCard, GainRagePointCard, AttackCard, ClearCard
}

[CreateAssetMenu(fileName = "CardData", menuName = "Scriptable Objects/CardData")]
public class CardData : ScriptableObject
{
    [SerializeField] private CardType _cardType;
    public CardType CardType => _cardType;
    [SerializeField] private int _magicPointCost;
    public int MagicPointCost => _magicPointCost;
    [SerializeField] private int _ragePointCost;
    public int RagePointCost => _ragePointCost;
    [SerializeField] private Sprite _cardSprite;
    public Sprite CardSprite => _cardSprite;
    [SerializeField] CardEffectSO _cardEffectSO;
    public CardEffectSO CardEffectSO => _cardEffectSO;
    [SerializeField] private string _description;
    public string Description => _description;

    public bool CanUse(int currentMagicPoint, int currentRagePoint)
    {
        return currentMagicPoint >= _magicPointCost && currentRagePoint >= _ragePointCost;
    }

}
