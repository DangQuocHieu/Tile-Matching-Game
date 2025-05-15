using UnityEngine;

public class CardSelectionScreen : UIScreen
{

    public override void Hide()
    {
        gameObject.SetActive(false);
    }

    public override void Show()
    {
        gameObject.SetActive(true);
    }
}
