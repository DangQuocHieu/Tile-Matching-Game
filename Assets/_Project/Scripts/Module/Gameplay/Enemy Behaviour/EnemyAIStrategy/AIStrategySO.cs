using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class AIStrategySO : ScriptableObject, IAIStrategy
{
    public abstract Tuple<GameObject, GameObject> FindBestMove(List<Tuple<GameObject, GameObject>> validMoves);
    public abstract CardData FindBestCard(List<CardData> usableCards);
}
