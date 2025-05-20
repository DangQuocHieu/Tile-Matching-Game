using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class AIStrategySO : ScriptableObject, IAIStrategy
{
    protected Dictionary<DiamondType, int> _scores = new Dictionary<DiamondType, int>();
    public abstract Tuple<GameObject, GameObject> FindBestMove(List<Tuple<GameObject, GameObject>> validMoves);
    public abstract CardData FindBestCard(List<CardData> usableCards);
}
