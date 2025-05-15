using UnityEngine;

[CreateAssetMenu(fileName = "CardData", menuName = "Scriptable Objects/CardData")]
public class CardData : ScriptableObject
{
    [SerializeField] private int _manaPointToUse;
    public int ManaPointToUse => _manaPointToUse;
    [SerializeField] private int _ragePointToUse;
    public int RagePointToUse => _ragePointToUse;
    [SerializeField] private Sprite _cardSprite;
    public Sprite CardSprite => _cardSprite;
    [SerializeField] CardEffectSO _cardEffectSO;
    public CardEffectSO CardEffectSO => _cardEffectSO;
    [SerializeField] DiamondType _type;
    public DiamondType Type => _type;
    [SerializeField] private string _description;
    public string Description => _description;
}
