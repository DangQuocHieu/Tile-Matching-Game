public class EnemyStat : UnitStatHandler, IMessageHandle
{

    void OnEnable()
    {
        MessageManager.AddSubcriber(GameMessageType.OnEnemyApplyEffect, this);
    }

    void OnDisable()
    {
        MessageManager.RemoveSubcriber(GameMessageType.OnEnemyApplyEffect, this);
    }


    public void Handle(Message message)
    {
        switch(message.type)
        {
            case GameMessageType.OnEnemyApplyEffect:
                DiamondType diamondType = (DiamondType)message.data[0];
                int counter = (int)message.data[1];
                ApplyEffect(diamondType, counter);
                break;
        }
    }

    public override void ApplyEffect(DiamondType type, int counter)
    {
        switch(type)
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
                break;
            case DiamondType.Shield:
                break;
            case DiamondType.Steal:
                break;
        }
    }
}
