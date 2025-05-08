using System.Collections;
using UnityEngine;

public interface IEffect
{
    public IEnumerator ApplyEffect(UnitStatHandler statHandler, int value);
}
