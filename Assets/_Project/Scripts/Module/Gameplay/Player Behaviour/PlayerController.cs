using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : Singleton<PlayerController>, IMessageHandle
{
    [SerializeField] private GameObject _previousDiamond;
    [SerializeField] private GameObject _currentDiamond;

    [SerializeField] private bool _disableControl = true;
    void OnEnable()
    {
        MessageManager.AddSubcriber(GameMessageType.OnSwappedFail, this);
        MessageManager.AddSubcriber(GameMessageType.OnPlayerTurnStart, this);
        MessageManager.AddSubcriber(GameMessageType.OnEnemyTurnStart, this);
    }

    void OnDisable()
    {
        MessageManager.RemoveSubcriber(GameMessageType.OnSwappedFail, this);
        MessageManager.RemoveSubcriber(GameMessageType.OnPlayerTurnStart, this);
        MessageManager.RemoveSubcriber(GameMessageType.OnEnemyTurnStart, this);
    }

    void Update()
    {
        HandleDiamondSelection();
    }


    private void HandleDiamondSelection()
    {
        if(_disableControl) return;
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector3.zero);
            if (hit.collider != null)
            {
                _previousDiamond = _currentDiamond;
                _currentDiamond = hit.collider.gameObject;
            }
            if(_previousDiamond != null && _currentDiamond != null && _previousDiamond != _currentDiamond)
            {
                //Valid move, prevent player control
                DisableControl();
            }
            DiamondController.Instance.SwapDiamond(_previousDiamond, _currentDiamond);
        }
    }
    public void Handle(Message message)
    {
        switch (message.type)
        {
            case GameMessageType.OnSwappedFail:
                _previousDiamond = default;
                _currentDiamond = default;
                EnableControl();
                break;
            case GameMessageType.OnEnemyTurnStart:
                DisableControl();
                break;
            case GameMessageType.OnPlayerTurnStart:
                EnableControl();
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
