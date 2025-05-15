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
        MessageManager.AddSubscriber(GameMessageType.OnDiamondSwappedFail, this);
        MessageManager.AddSubscriber(GameMessageType.OnDiamondSwapped, this);
    }

    void OnDisable()
    {
        MessageManager.RemoveSubscriber(GameMessageType.OnDiamondSwappedFail, this);      
        MessageManager.RemoveSubscriber(GameMessageType.OnDiamondSwapped, this);
    }

    public void Handle(Message message)
    {
        switch(message.type)
        {
            case GameMessageType.OnDiamondSwappedFail:
                UnHighlight();
                break;
            case GameMessageType.OnDiamondSwapped:
                UnHighlight();
                break;

        }
    }

    public void Highlight(Vector3 position)
    {
        _spriteRenderer.enabled = true;
        transform.position = position;
    }

    public void UnHighlight()
    {
        _spriteRenderer.enabled = false;
    }
}
