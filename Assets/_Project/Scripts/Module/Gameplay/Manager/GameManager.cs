using UnityEngine;

public class GameManager : Singleton<GameManager>, IMessageHandle
{
    void OnEnable()
    {
        MessageManager.AddSubscriber(GameMessageType.OnGameUnitDied, this);
    }

    void OnDisable()
    {
        MessageManager.RemoveSubscriber(GameMessageType.OnGameUnitDied, this);
    }

    public void Handle(Message message)
    {
        switch (message.type)
        {
            case GameMessageType.OnGameUnitDied:
                Side side = (Side)message.data[0];
                if (side == PlayerController.Instance.PlayerSide)
                {
                    OnGameLose();
                }
                else
                {
                    OnGameWin();
                }
                break;
        }
    }

    private void OnGameWin()
    {
        MessageManager.SendMessage(new Message(GameMessageType.OnGameWin));
    }

    private void OnGameLose()
    {
        MessageManager.SendMessage(new Message(GameMessageType.OnGameLose));
    }
}
