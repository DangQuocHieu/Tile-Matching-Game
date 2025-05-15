using UnityEngine;

public abstract class UIScreen : MonoBehaviour
{
    [SerializeField] protected ScreenKey _key;
    public ScreenKey Key => _key;
    public abstract void Show();
    public abstract void Hide();
}
