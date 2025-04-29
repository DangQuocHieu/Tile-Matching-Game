using UnityEngine;

public class GameUnit : MonoBehaviour
{
    private UnitStatHandler _statHandler;
    public UnitStatHandler StatHandler => _statHandler;
    private EffectHandler _effectHandler;
    public EffectHandler EffectHandler => _effectHandler;
    [SerializeField] private Side _side;
    public Side Side => _side;
    void Awake()
    {
        _statHandler = GetComponent<UnitStatHandler>();
        _effectHandler = GetComponent<EffectHandler>();
    }

    public bool IsInTurn()
    {
        Side currentSide = TurnManager.Instance.CurrentSide;
        return _side == currentSide;
    }

}
