using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EnemyController : Singleton<EnemyController>, IMessageHandle
{

    void OnEnable()
    {
        MessageManager.AddSubcriber(GameMessageType.OnEnemyTurnStart, this);
    }

    void OnDisable()
    {
        MessageManager.RemoveSubcriber(GameMessageType.OnEnemyTurnStart, this);
    }
    private void PerformAction()
    {
        List<Tuple<GameObject, GameObject>> validMoves = BoardManager.Instance.GenerateValidMoves();
        Tuple<GameObject, GameObject> randomMoves = validMoves[UnityEngine.Random.Range(0, validMoves.Count)];
        DiamondController.Instance.SwapDiamond(randomMoves.Item1, randomMoves.Item2);
    }

    private void PerformActionWithDelay(float delayDuration)
    {
        Invoke("PerformAction", delayDuration);
    }

    public void Handle(Message message)
    {
        switch(message.type)
        {
            case GameMessageType.OnEnemyTurnStart:
                PerformActionWithDelay(2f);
                break;
        }
    }
}
