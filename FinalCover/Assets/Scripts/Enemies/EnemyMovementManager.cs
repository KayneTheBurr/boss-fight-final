using UnityEngine;

public class EnemyMovementManager : CharacterMovementManager
{
    EnemyCharacterManager enemy;

    protected override void Awake()
    {
        base.Awake();
        enemy = GetComponent<EnemyCharacterManager>();
    }
    protected override void Update()
    {
        base.Update();
        UpdateAnimatorMovementValues();
    }
    public void RotateTowardsAgent(EnemyCharacterManager enemy)
    {
        if (enemy.enemyMovementManager.isMoving.GetBool()) //if the ai character is moving do this 
        {
            enemy.transform.rotation = enemy.navMeshAgent.transform.rotation;
        }
    }
    private void UpdateAnimatorMovementValues()
    {
        if (enemy.navMeshAgent == null) return;

        Vector3 worldVel = enemy.navMeshAgent.desiredVelocity;

        // if not moving, set all to zero
        if (worldVel.sqrMagnitude < 0.001f)
        {
            enemy.horizontalMovement = 0;
            enemy.verticalMovement = 0;
            enemy.moveAmount = 0;
            enemy.enemyAnimationManager.UpdateAnimatorMovementParameters(0, 0, false);
            return;
        }

        // convert to local for left/right and fwd/back
        Vector3 localVel = enemy.transform.InverseTransformDirection(worldVel.normalized);

        enemy.verticalMovement = Mathf.Clamp(localVel.z, -1f, 1f);
        enemy.horizontalMovement = Mathf.Clamp(localVel.x, -1f, 1f);
        enemy.moveAmount = Mathf.Clamp01(Mathf.Abs(enemy.verticalMovement) + Mathf.Abs(enemy.horizontalMovement));

        enemy.enemyAnimationManager.UpdateAnimatorMovementParameters(
            enemy.horizontalMovement, enemy.verticalMovement, false);
    }
}
