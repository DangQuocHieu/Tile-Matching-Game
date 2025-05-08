using UnityEngine;

[CreateAssetMenu(fileName = "CharacterSelectionConfig", menuName = "Scriptable Objects/CharacterSelectionConfig")]
public class CharacterSelectionConfig : ScriptableObject
{
    [SerializeField] private Sprite _characterIcon;
    public Sprite CharacterIcon => _characterIcon;
    [SerializeField] private Sprite _characterImage;
    public Sprite CharacterImage => _characterImage;
    [SerializeField] private UnitStat _stat;
    public UnitStat Stat => _stat;
}
