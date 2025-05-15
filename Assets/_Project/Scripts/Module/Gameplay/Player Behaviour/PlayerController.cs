using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : Singleton<PlayerController>, IMessageHandle
{
    [SerializeField] private GameObject _previousDiamond;
    public GameObject PreviousDiamond => _previousDiamond;
    [SerializeField] private GameObject _currentDiamond;
    public GameObject CurrentDiamond => _currentDiamond;
    [SerializeField] private bool _disableControl = true;
    [SerializeField] private Side _side = Side.LeftSide;
    [SerializeField] LayerMask _diamondLayer;

    private void OnEnable()
    {
        MessageManager.AddSubscriber(GameMessageType.OnTurnStart, this);
        MessageManager.AddSubscriber(GameMessageType.OnDiamondSwappedFail, this);
        MessageManager.AddSubscriber(GameMessageType.OnDiamondSwapped, this);
        MessageManager.AddSubscriber(GameMessageType.OnProcessBoardStart, this);
    }

    private void OnDisable()
    {
        MessageManager.RemoveSubscriber(GameMessageType.OnTurnStart, this);
        MessageManager.RemoveSubscriber(GameMessageType.OnDiamondSwappedFail, this);
        MessageManager.RemoveSubscriber(GameMessageType.OnDiamondSwapped, this);
        MessageManager.RemoveSubscriber(GameMessageType.OnProcessBoardStart, this);
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
                ResetDiamond();
                EnableControl();
                break;
            case GameMessageType.OnTurnStart:
                ResetDiamond();
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
            case GameMessageType.OnProcessBoardStart:
                DisableControl();
                break;
        }
    }

    public void DisableControl()
    {
        _disableControl = true;
    }

    private void EnableControl()
    {
        _disableControl = false;
    }

    public void ResetDiamond()
    {
        _previousDiamond = default;
        _currentDiamond = default;
    }


}
