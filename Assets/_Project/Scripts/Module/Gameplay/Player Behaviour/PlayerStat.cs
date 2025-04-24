using UnityEditor;
using UnityEngine;

public class PlayerStat : UnitStatHandler, IMessageHandle
{

    
    void OnEnable()
    {
        MessageManager.AddSubcriber(GameMessageType.OnPlayerApplyEffect, this);
    }

    void OnDisable()
    {
        MessageManager.RemoveSubcriber(GameMessageType.OnPlayerApplyEffect, this);
    }

    public override void ApplyEffect(DiamondType type, int counter)
    {
        switch (type)
        {
            case DiamondType.Attack:
                break;
            case DiamondType.Health:
                AddHealthPoint(counter);
                break;
            case DiamondType.Mana:
                AddManaPoint(counter);
                break;
            case DiamondType.Rage:
                AddRagePoint(counter);
                break;
            case DiamondType.Shield:
                AddShieldPoint(counter);
                break;
            case DiamondType.Steal:
                break;
        }
    }
    public void Handle(Message message)
    {
        switch (message.type)
        {
            case GameMessageType.OnPlayerApplyEffect:
                DiamondType type = (DiamondType)message.data[0];
                int counter = (int)message.data[1];
                ApplyEffect(type, counter);
                break;
        }
    }
}
