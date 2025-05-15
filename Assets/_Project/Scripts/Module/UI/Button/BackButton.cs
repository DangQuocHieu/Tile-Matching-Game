using UnityEngine;

public class BackButton : MonoBehaviour
{
    public void GoBack()
    {
        ScreenManager.Instance.GoBack();
    }
}
