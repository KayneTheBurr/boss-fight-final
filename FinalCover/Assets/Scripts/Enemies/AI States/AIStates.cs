using UnityEngine;

public class AIStates : ScriptableObject
{
    public virtual AIStates Tick(EnemyCharacterManager enemy)
    {


        return this;
    }
    public virtual AIStates SwitchState(EnemyCharacterManager enemy, AIStates newState)
    {
        ResetStateFlags(enemy);
        return newState;
    }
    protected virtual void ResetStateFlags(EnemyCharacterManager enemy)
    {

    }
}
