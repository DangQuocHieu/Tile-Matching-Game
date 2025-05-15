using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : Singleton<EnemyController>, IMessageHandle
{
    [SerializeField] private float minDelay = 2f;
    [SerializeField] private float maxDelay = 5f;

    [SerializeField] private Side _side = Side.RightSide;
    [SerializeField] private AIStrategySO[] _strategies;
    private void OnEnable()
    {
        MessageManager.AddSubscriber(GameMessageType.OnTurnStart, this);
    }

    private void OnDisable()
    {
        MessageManager.RemoveSubscriber(GameMessageType.OnTurnStart, this);
    }
    private void PerformAction()
    {
        List<Tuple<GameObject, GameObject>> validMoves = BoardManager.Instance.GenerateValidMoves();
        Tuple<GameObject, GameObject> bestMove = _strategies[0].FindBestMove(validMoves);
        DiamondController.Instance.SwapDiamond(bestMove.Item1, bestMove.Item2);
    }

    public void Handle(Message message)
    {
        switch (message.type)
        {
            case GameMessageType.OnTurnStart:
                Side currentSide = (Side)message.data[0];
                if (_side == currentSide)
                {
                    Invoke("PerformAction", UnityEngine.Random.Range(minDelay, maxDelay + 1));
                }
                break;
        }
    }
}
