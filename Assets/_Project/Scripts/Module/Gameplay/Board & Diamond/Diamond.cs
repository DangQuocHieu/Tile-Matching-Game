using UnityEngine;
using UnityEngine.Tilemaps;
public enum DiamondType
{
    None = 0, Attack = 1, Mana = 2, Rage = 3, Steal = 4, Health = 5, Shield = 6
}
public class Diamond : MonoBehaviour
{
    [SerializeField] private DiamondType _diamondType;
    public DiamondType DiamondType => _diamondType;
    // private int _xPosition;
    // private int _yPosition;
    // public int XPosition 
    // {
    //     get {return _xPosition;}
    //     set {_xPosition = value;}
    // }

    // public int yPosition
    // {
    //     get {return _yPosition;}
    //     set {_yPosition = value;}
    // }

}
