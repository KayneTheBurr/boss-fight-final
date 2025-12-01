using Unity.VisualScripting;
using UnityEngine;

public class EnemyMovementManager : CharacterMovementManager
{
    EnemyCharacterManager enemy;

    [SerializeField] float rotationSpeed = 5f;

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
    public void RotateWithMovement(EnemyCharacterManager enemy)
    {
        if (enemy.enemyMovementManager.isMoving.GetBool()) //if the ai character is moving do this 
        {
            enemy.transform.rotation = enemy.navMeshAgent.transform.rotation;
        }
    }
    public void RotateToFaceTarget(EnemyCharacterManager enemy)
    {
        var cm = enemy.enemyCombatManager;

        if (cm == null || cm.currentTarget == null) return;

        if (!enemy.canRotate) return;

        Vector3 dir = cm.currentTarget.transform.position - enemy.transform.position;
        dir.y = 0f;

        if (dir.sqrMagnitude < 0.0001f)
            return;

        dir.Normalize();
        Quaternion targetRot = Quaternion.LookRotation(dir);
        Quaternion finalRot = Quaternion.Slerp(
            enemy.transform.rotation,
            targetRot,
            rotationSpeed * Time.deltaTime
        );

        enemy.transform.rotation = finalRot;
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
