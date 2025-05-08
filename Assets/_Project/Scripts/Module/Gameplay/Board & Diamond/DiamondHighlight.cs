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
        MessageManager.AddSubcriber(GameMessageType.OnDiamondSwappedFail, this);
        MessageManager.AddSubcriber(GameMessageType.OnDiamondSwapped, this);
    }

    void OnDisable()
    {
        MessageManager.RemoveSubcriber(GameMessageType.OnDiamondSwappedFail, this);      
        MessageManager.RemoveSubcriber(GameMessageType.OnDiamondSwapped, this);
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
