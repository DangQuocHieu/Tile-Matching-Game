using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>, IMessageHandle
{
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        MessageManager.AddSubscriber(GameMessageType.OnGameUnitDied, this);
        MessageManager.AddSubscriber(GameMessageType.OnGamePaused, this);
        MessageManager.AddSubscriber(GameMessageType.OnGameContinued, this);

    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        MessageManager.RemoveSubscriber(GameMessageType.OnGameUnitDied, this);
        MessageManager.RemoveSubscriber(GameMessageType.OnGamePaused, this);
        MessageManager.RemoveSubscriber(GameMessageType.OnGameContinued, this);
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
            case GameMessageType.OnGamePaused:
                Time.timeScale = 0;
                break;
            case GameMessageType.OnGameContinued:
                Time.timeScale = 1;
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

    private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        Time.timeScale = 1f;
    }
}
