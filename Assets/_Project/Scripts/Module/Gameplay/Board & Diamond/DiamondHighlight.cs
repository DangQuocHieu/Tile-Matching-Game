using UnityEngine;

public class DiamondHighlight : Singleton<DiamondHighlight>, IMessageHandle
{
    private SpriteRenderer _spriteRenderer;

    void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }
    void OnEnable()
    {
        MessageManager.AddSubcriber(GameMessageType.OnDiamondSelected, this);
        MessageManager.AddSubcriber(GameMessageType.OnSwappedFail, this);
    }

    void OnDisable()
    {
        MessageManager.RemoveSubcriber(GameMessageType.OnDiamondSelected, this);
        MessageManager.RemoveSubcriber(GameMessageType.OnSwappedFail, this);      
    }

    public void Handle(Message message)
    {
        switch(message.type)
        {
            case GameMessageType.OnDiamondSelected:
                Vector3 position = (Vector3)message.data[0];
                Highlight(position);
                break;
            case GameMessageType.OnSwappedFail:
                UnHighlight();
                break;

        }
    }

    private void Highlight(Vector3 position)
    {
        _spriteRenderer.enabled = true;
        transform.position = position;
    }

    public void UnHighlight()
    {
        _spriteRenderer.enabled = false;
    }
}
