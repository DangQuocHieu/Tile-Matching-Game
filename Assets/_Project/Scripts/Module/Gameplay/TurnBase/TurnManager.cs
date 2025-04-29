using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
public enum Side
{
    LeftSide, RightSide
    //LeftSide for player and RightSide for enemy
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

    protected override void Awake()
    {
        base.Awake();
        StartCoroutine(LeftTurnCoroutine());

    }

    private void OnEnable()
    {
        MessageManager.AddSubcriber(GameMessageType.OnCurrentTurnPaused, this);
        MessageManager.AddSubcriber(GameMessageType.OnCurrentTurnEnd, this);
    }

    private void OnDisable()
    {
        MessageManager.RemoveSubcriber(GameMessageType.OnCurrentTurnPaused, this);
        MessageManager.RemoveSubcriber(GameMessageType.OnCurrentTurnEnd, this);
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
        StartNextTurn();
    }

    private IEnumerator TurnStartDelay()
    {
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

    private void PauseCurrentTurn()
    {
        inProgress = false;
    }
    
    public void Handle(Message message)
    {
        switch(message.type)
        {
            case GameMessageType.OnCurrentTurnPaused:
                PauseCurrentTurn();
                break;
            case GameMessageType.OnCurrentTurnEnd:
                EndCurrentTurn();
                Invoke("StartNextTurn", 2f);
                break;
        }
    }

    public void GetEnemyUnit(Side side)
    {

    }
}
