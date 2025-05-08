using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : Singleton<EnemyController>, IMessageHandle
{
    [SerializeField] private float minDelay = 2f;
    [SerializeField] private float maxDelay = 5f;

    [SerializeField] private Side _side = Side.RightSide;
    private void OnEnable()
    {
        MessageManager.AddSubcriber(GameMessageType.OnTurnStart, this);
    }

    private void OnDisable()
    {
        MessageManager.RemoveSubcriber(GameMessageType.OnTurnStart, this);
    }
    private void PerformAction()
    {
        List<Tuple<GameObject, GameObject>> validMoves = BoardManager.Instance.GenerateValidMoves();
        Tuple<GameObject, GameObject> randomMoves = validMoves[UnityEngine.Random.Range(0, validMoves.Count)];
        DiamondController.Instance.SwapDiamond(randomMoves.Item1, randomMoves.Item2);
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
