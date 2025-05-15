using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
public enum Side
{
    LeftSide, RightSide, None
}
public class TurnManager : Singleton<TurnManager>, IMessageHandle
{
    [SerializeField] private Side _currentSide;
    public Side CurrentSide => _currentSide;
    [SerializeField] private float _turnDuration;
    public float TurnDuration => _turnDuration;
    [SerializeField] private float _turnStartDelay;
    private Coroutine _currentTurnCoroutine;
    private bool inProgress = true;

    private void OnEnable()
    {
        MessageManager.AddSubscriber(GameMessageType.OnCharacterLoaded, this);
        MessageManager.AddSubscriber(GameMessageType.OnCurrentTurnPaused, this);
        MessageManager.AddSubscriber(GameMessageType.OnCurrentTurnEnd, this);
        MessageManager.AddSubscriber(GameMessageType.OnApplyCardEffectEnd, this);
    }

    private void OnDisable()
    {
        MessageManager.RemoveSubscriber(GameMessageType.OnCharacterLoaded, this);
        MessageManager.RemoveSubscriber(GameMessageType.OnCurrentTurnPaused, this);
        MessageManager.RemoveSubscriber(GameMessageType.OnCurrentTurnEnd, this);
        MessageManager.RemoveSubscriber(GameMessageType.OnApplyCardEffectEnd, this);
    }

    public IEnumerator LeftTurnCoroutine()
    {
        _currentSide = Side.LeftSide;
        yield return TurnStartDelay();
        yield return _currentTurnCoroutine = StartCoroutine(TurnTimer());
    }

    public IEnumerator RightTurnCoroutine()
    {
        _currentSide = Side.RightSide;
        yield return TurnStartDelay();
        yield return _currentTurnCoroutine = StartCoroutine(TurnTimer());
    }

    private IEnumerator TurnTimer()
    {
        MessageManager.SendMessage(new Message(GameMessageType.OnTurnStart, new object[] {CurrentSide}));
        float timeRemaining = _turnDuration;
        while (timeRemaining >= 0)
        {
            if (inProgress)
            {
                MessageManager.SendMessage(new Message(GameMessageType.OnTurnInProgress, new object[] { timeRemaining }));
                timeRemaining -= Time.deltaTime;
            }
            yield return null;
        }
        EndCurrentTurn();
        Invoke("StartNextTurn", 2f);
    }

    private IEnumerator TurnStartDelay()
    {
        Debug.Log("SEND MESSAGE");
        MessageManager.SendMessage(new Message(GameMessageType.OnTurnStartDelay, new object[] { _turnDuration, _turnStartDelay, _currentSide }));
        yield return new WaitForSeconds(_turnStartDelay);
    }
    private void EndCurrentTurn()
    {
        if (_currentTurnCoroutine != null)
        {
            StopCoroutine(_currentTurnCoroutine);
            _currentTurnCoroutine = null;
        }
    }

    private void StartNextTurn()
    {
        inProgress = true;
        if (_currentSide == Side.LeftSide)
        {
            StartCoroutine(RightTurnCoroutine());
        }
        else
        {
            StartCoroutine(LeftTurnCoroutine());
        }
    }

    public void PauseCurrentTurn()
    {
        inProgress = false;
    }
    
    public void ContinueCurrentTurn()
    {
        inProgress = true;
    }
    public void Handle(Message message)
    {
        switch(message.type)
        {
            case GameMessageType.OnCharacterLoaded:
                StartCoroutine(LeftTurnCoroutine());
                break;
            case GameMessageType.OnCurrentTurnPaused:
                PauseCurrentTurn();
                break;
            case GameMessageType.OnCurrentTurnEnd:
                EndCurrentTurn();
                Invoke("StartNextTurn", 2f);
                break;
            case GameMessageType.OnApplyCardEffectEnd:
                ContinueCurrentTurn();
                break;
        }
    }
}
