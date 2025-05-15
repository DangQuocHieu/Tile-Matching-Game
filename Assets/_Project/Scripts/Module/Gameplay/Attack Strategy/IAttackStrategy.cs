using System.Collections;

public interface IAttackStrategy
{
    public IEnumerator Execute(GameUnit attacker, GameUnit target);
}

