using UnityEngine;
using UnityEngine.UI;

public class BackgroundSelector : MonoBehaviour
{
    [SerializeField] private Button _button;
    [SerializeField] private Image _image;
    public Image Image => _image;
    public void Init(Sprite sprite)
    {
        _image.sprite = sprite;
        AddButtonListener();
        
    }

    private void AddButtonListener()
    {
        _button.onClick.AddListener(()=>{
            BackgroundSelectionController.Instance.HandleBackgroundSelection(this);
        });
    }

    
}
