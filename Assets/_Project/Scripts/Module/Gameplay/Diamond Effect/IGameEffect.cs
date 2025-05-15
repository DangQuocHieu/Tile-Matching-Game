using System.Collections;
using UnityEngine;
public interface IGameEffect 
{
    public IEnumerator Execute(int value);
    public IEnumerator Execute(UnitStatHandler handler, int value);

}
