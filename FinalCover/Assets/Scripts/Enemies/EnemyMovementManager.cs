using UnityEngine;

public class EnemyMovementManager : CharacterMovementManager
{
    EnemyCharacterManager enemy;

    protected override void Awake()
    {
        base.Awake();

        enemy = GetComponent<EnemyCharacterManager>();
    }
    public void RotateTowardsAgent(EnemyCharacterManager enemy)
    {
        if (enemy.enemyMovementManager.isMoving.GetBool()) //if the ai character is moving do this 
        {
            enemy.transform.rotation = enemy.navMeshAgent.transform.rotation;
        }
    }
}
