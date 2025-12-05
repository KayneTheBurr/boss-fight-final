using System.Collections.Generic;
using UnityEngine;

public class AssassinEnemyCombatManager : EnemyCombatManager
{
    [Header("Assassin Movement")]
    public float idealRange = 4f;
    public float minRange = 2.5f;
    public float maxRange = 6f;
    public float circleOffsetDistance = 2f;
    public float circleDirChangeInterval = 3f;

    [Header("Assassin Dodge Tuning")]
    [SerializeField] float reactionTime = 1f;
    [SerializeField] float dodgeChanceOnPlayerAttack = 0.3f;
    public float dodgeCooldown = 1.5f;
    public bool hasAttemptedDodgeThisAttack = false;

    [HideInInspector] public float circleDir = 1f;
    private float circleTimer = 0f;
    private float dodgeCooldownTimer = 0f;
    public float reactionTimer = 0f;

    protected override void Start()
    {
        base.Start();
        enemy.navMeshAgent.updateRotation = false;
    }
    protected override void LateUpdate()
    {
        base.LateUpdate();

        if (dodgeCooldownTimer > 0)
            dodgeCooldownTimer -= Time.deltaTime;

        if(reactionTimer > 0)
            reactionTimer -= Time.deltaTime;

        else if(reactionTimer <= 0)
            reactionTimer = reactionTime;
        
    }
    public void HandleAssassinMovement(EnemyCharacterManager enemy)
    {
        if (currentTarget == null) return;

        targetDirection /= distanceFromTarget;

        // randomly change circling dir
        circleTimer -= Time.deltaTime;
        if (circleTimer <= 0f)
        {
            circleTimer = circleDirChangeInterval;
            if (Random.value < 0.5f)
                circleDir *= -1f;
        }

        // perpendicular vector in circling direction
        Vector3 perp = new Vector3(-targetDirection.z, 0, targetDirection.x) * circleDir;

        float radialOffset = 0f;
        if (distanceFromTarget < minRange) radialOffset = -1f;     // too close, back up
        else if (distanceFromTarget > maxRange) radialOffset = 1f; // too far, close in

        Vector3 desiredPos =
            currentTarget.transform.position
            - targetDirection * (idealRange + radialOffset)
            + perp * circleOffsetDistance;

        enemy.navMeshAgent.SetDestination(desiredPos);
        enemy.enemyMovementManager.RotateToFaceTarget(enemy);
    }

    public virtual bool TryDodge(EnemyCharacterManager enemy)
    {
        if (dodgeCooldownTimer > 0f) return false;
        var dodgeRoll = Random.value;
        //Debug.Log(dodgeRoll);
        if (dodgeRoll > dodgeChanceOnPlayerAttack) return false;

        // pick dodge direction based on viewableAngle / some rule
        // animation and root motion handled by animation system

        dodgeCooldownTimer = dodgeCooldown;
        return true;
    }
    public virtual void PerformDodge(EnemyCharacterManager enemy)
    {
        //Debug.Log("I'm trying to dodge!");
    }
}
