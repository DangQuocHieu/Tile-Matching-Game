using System.Collections;
using System.Threading;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public enum TurnType
{
    PlayerTurn, EnemyTurn
}
public class TurnManager : Singleton<TurnManager>
{
    [SerializeField] private TurnType _currentTurn;
    public TurnType CurrentTurn => _currentTurn;
    [SerializeField] private float _turnDuration;
    public float TurnDuration => _turnDuration;
    [SerializeField] private float _turnStartDelay;
    private Coroutine _currentTurnCoroutine;
    private bool inProgress = true;

    protected override void Awake()
    {
        base.Awake();
        StartCoroutine(PlayerTurnCoroutine());

    }
    public IEnumerator PlayerTurnCoroutine()
    {
        _currentTurn = TurnType.PlayerTurn;
        yield return TurnStartDelay();
        MessageManager.SendMessage(new Message(GameMessageType.OnPlayerTurnStart));
        yield return _currentTurnCoroutine = StartCoroutine(TurnTimer());
    }

    public IEnumerator EnemyTurnCoroutine()
    {
        _currentTurn = TurnType.EnemyTurn;
        yield return TurnStartDelay();
        MessageManager.SendMessage(new Message(GameMessageType.OnEnemyTurnStart));
        yield return _currentTurnCoroutine = StartCoroutine(TurnTimer());
    }

    private IEnumerator TurnTimer()
    {
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
        MessageManager.SendMessage(new Message(GameMessageType.OnTurnStartDelay, new object[] { _turnDuration, _turnStartDelay, _currentTurn }));
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
        if (_currentTurn == TurnType.PlayerTurn)
        {
            StartCoroutine(EnemyTurnCoroutine());
        }
        else
        {
            StartCoroutine(PlayerTurnCoroutine());
        }
    }
}
