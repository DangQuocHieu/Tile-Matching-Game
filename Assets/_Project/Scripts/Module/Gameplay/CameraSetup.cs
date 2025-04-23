using UnityEngine;

public class CameraSetup : MonoBehaviour
{
    void Start()
    {
        float boardWidth = BoardManager.Instance.BoardWidth / 2 - 0.5f;
        float BoardHeight = BoardManager.Instance.BoardHeight / 2;
        transform.position = new Vector3(boardWidth, BoardHeight, transform.position.z);
    }
}
