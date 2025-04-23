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
        throw new System.NotImplementedException();
    }

    public void Handle(Message message)
    {
        switch(message.type)
        {
            case GameMessageType.OnPlayerApplyEffect:
                DiamondType type = (DiamondType)message.data[0];
                int counter = (int)message.data[1];
                ApplyEffect(type, counter);
                break;
        }
    }
}
