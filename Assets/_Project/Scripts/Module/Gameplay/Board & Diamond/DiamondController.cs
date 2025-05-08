using UnityEngine;

public class DiamondController : Singleton<DiamondController>
{
    public void SwapDiamond(GameObject previousDiamond, GameObject currentDiamond)
    {
        if(previousDiamond == default) return;
        if (AreNeighbors(previousDiamond.transform.position, currentDiamond.transform.position))
        {
            MessageManager.SendMessage(new Message(GameMessageType.OnDiamondSwapped, new object[] { previousDiamond, currentDiamond }));
        }        
    }

    public void SelectDiamond(Vector3 diamondPosition)
    {
        MessageManager.SendMessage(new Message(GameMessageType.OnDiamondSelected, new object[] { diamondPosition }));
    }

    private bool AreNeighbors(Vector3 a, Vector3 b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y) == 1;
    }

}
