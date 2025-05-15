using System.Collections;
using UnityEngine;

public interface ICardEffect
{
    public abstract IEnumerator Activate();
    public abstract void OnComplete(GameObject game);
}
