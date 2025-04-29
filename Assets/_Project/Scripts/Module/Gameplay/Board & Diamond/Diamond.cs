using UnityEngine;
using UnityEngine.Tilemaps;
public enum DiamondType
{
    None = 0, Mana = 1, Rage = 2, Health = 3, Shield = 4, Steal = 5, Attack = 6
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
