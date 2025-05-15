using UnityEngine;
using UnityEngine.UI;

public class GameplayBackground : MonoBehaviour
{
    [SerializeField] private Image _image;
    void Start()
    {
        _image.sprite = BackgroundSelectionController.Instance.CurrentSelector.Image.sprite;
    }
}
