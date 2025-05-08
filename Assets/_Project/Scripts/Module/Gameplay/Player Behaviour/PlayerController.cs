using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : Singleton<PlayerController>, IMessageHandle
{
    [SerializeField] private GameObject _previousDiamond;
    [SerializeField] private GameObject _currentDiamond;
    [SerializeField] private bool _disableControl = true;
    [SerializeField] private Side _side = Side.LeftSide;
    [SerializeField] LayerMask _diamondLayer;

    private void OnEnable()
    {
        MessageManager.AddSubcriber(GameMessageType.OnTurnStart, this);
        MessageManager.AddSubcriber(GameMessageType.OnDiamondSwappedFail, this);
        MessageManager.AddSubcriber(GameMessageType.OnDiamondSwapped, this);
    }

    private void OnDisable()
    {
        MessageManager.RemoveSubcriber(GameMessageType.OnTurnStart, this);
        MessageManager.RemoveSubcriber(GameMessageType.OnDiamondSwappedFail, this);
        MessageManager.RemoveSubcriber(GameMessageType.OnDiamondSwapped, this);
    }
    void Update()
    {
        HandleDiamondSelection();
    }


    private void HandleDiamondSelection()
    {
        if (_disableControl) return;
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector3.zero, _diamondLayer);
            if (hit.collider != null)
            {
                _previousDiamond = _currentDiamond;
                _currentDiamond = hit.collider.gameObject;
                DiamondHighlight.Instance.Highlight(_currentDiamond.transform.position);
            }
            else
            {
                DiamondHighlight.Instance.UnHighlight();
            }
            DiamondController.Instance.SwapDiamond(_previousDiamond, _currentDiamond);
        }
    }
    public void Handle(Message message)
    {
        switch (message.type)
        {
            case GameMessageType.OnDiamondSwappedFail:
                _previousDiamond = default;
                _currentDiamond = default;
                EnableControl();
                break;
            case GameMessageType.OnTurnStart:
                Side currentSide = (Side)message.data[0];
                if (_side == currentSide)
                {
                    EnableControl();
                }
                else
                {
                    DisableControl();
                }
                break;
            case GameMessageType.OnDiamondSwapped:
                DisableControl();
                break;
        }
    }

    private void DisableControl()
    {
        _disableControl = true;
    }

    private void EnableControl()
    {
        _disableControl = false;
    }
}
