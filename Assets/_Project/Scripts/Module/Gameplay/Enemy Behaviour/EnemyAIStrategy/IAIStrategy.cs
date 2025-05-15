using System;
using System.Collections.Generic;
using UnityEngine;

public interface IAIStrategy 
{
    public abstract Tuple<GameObject, GameObject> FindBestMove(List<Tuple<GameObject, GameObject>> validMoves);
}
