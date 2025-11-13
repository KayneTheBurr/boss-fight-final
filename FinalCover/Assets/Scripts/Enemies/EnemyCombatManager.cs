using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.Image;

public class EnemyCombatManager : CharacterCombatManager
{
    protected EnemyCharacterManager enemy;

    [Header("Action Recovery")]
    public float actionRecoveryTimer = 0;

    [Header("Target Info")]
    public float viewableAngle;
    public float distanceFromTarget;
    public Vector3 targetDirection;

    [Header("Detection")]
    [SerializeField] float detectionRadius = 10;
    public float minFOV = -35;
    public float maxFOV = 35;

    [Header("Attacks")]
    public List<EnemyAttackAction> availableAttacks = new();

    public Dictionary<EnemyAttackAction, float> cooldownTracker = new();


    protected override void Awake()
    {
        base.Awake();
        enemy = GetComponent<EnemyCharacterManager>();
    }
    protected override void Start()
    {
        foreach(var attack in enemy.combatStance.enemyCharacterAttacks)
        {
            availableAttacks.Add(attack);
            cooldownTracker.Add(attack, 0);
        }
    }
    public void FindATargetViaLineOfSight(EnemyCharacterManager aiCharacter)
    {
        if (currentTarget != null) return;

        Collider[] colliders = Physics.OverlapSphere(aiCharacter.transform.position, detectionRadius, WorldUtilityManager.instance.GetCharacterLayers());
        
        for (int i = 0; i < colliders.Length; i++)
        {
            CharacterManager targetCharacter = colliders[i].transform.GetComponent<CharacterManager>();

            if (targetCharacter == null) continue; //if not a character pass
            if (targetCharacter == aiCharacter) continue; //if its me pass
            if (targetCharacter.isDead) continue; //skip dead characters

            //check if we are on teh same "team"
            if (WorldUtilityManager.instance.CanIDamageThisTarget(aiCharacter.characterGroup, targetCharacter.characterGroup))
            {
                //if a potential target is found, is it in front of us?
                Vector3 targetsDirection = targetCharacter.transform.position - aiCharacter.transform.position;
                float angleOfPotentialTarget = Vector3.Angle(targetsDirection, aiCharacter.transform.forward);

                if (angleOfPotentialTarget > minFOV && angleOfPotentialTarget < maxFOV) //if in FOV
                {
                    
                    Debug.DrawLine(aiCharacter.characterCombatManager.lockOnTransform.position,
                        targetCharacter.characterCombatManager.lockOnTransform.position, Color.green);

                    //last, check for linecast if the target is obscured
                    if (Physics.Linecast(aiCharacter.characterCombatManager.lockOnTransform.position,
                            targetCharacter.characterCombatManager.lockOnTransform.position,
                            WorldUtilityManager.instance.GetEnviroLayers()))
                    {
                        Debug.DrawLine(aiCharacter.characterCombatManager.lockOnTransform.position,
                            targetCharacter.characterCombatManager.lockOnTransform.position, Color.red);
                    }
                    else
                    {
                        targetsDirection = targetCharacter.transform.position - transform.position;
                        viewableAngle = WorldUtilityManager.instance.GetAngleOfTarget(transform, targetsDirection);

                        //assign the target
                        aiCharacter.characterCombatManager.SetTarget(targetCharacter);

                        aiCharacter.animator.SetBool("InCombatStance", true);

                        //Once target is found, turn/pivot towards the target rather than slowly walking at an angle towards them 
                        PivotTowardsTarget(aiCharacter);
                    }
                }
            }
        }
    }

    public virtual void PivotTowardsTarget(EnemyCharacterManager aICharacter)
    {
        //play a pivot animation depending on viewabe angle of target character
        if (aICharacter.isPerformingAction) return;

        if (viewableAngle >= 30 && viewableAngle <= 60)
        {
            aICharacter.characterAnimationManager.PlayTargetActionAnimation("Turn_R45_01", true);
        }
        else if (viewableAngle <= -30 && viewableAngle >= -60)
        {
            aICharacter.characterAnimationManager.PlayTargetActionAnimation("Turn_L45_01", true);
        }
        else if (viewableAngle > 60 && viewableAngle <= 110)
        {
            aICharacter.characterAnimationManager.PlayTargetActionAnimation("Turn_R90_01", true);
        }
        else if (viewableAngle < -60 && viewableAngle >= -110)
        {
            aICharacter.characterAnimationManager.PlayTargetActionAnimation("Turn_L90_01", true);
        }
        else if (viewableAngle > 110 && viewableAngle <= 145)
        {
            aICharacter.characterAnimationManager.PlayTargetActionAnimation("Turn_R135_01", true);
        }
        else if (viewableAngle < -110 && viewableAngle >= -145)
        {
            aICharacter.characterAnimationManager.PlayTargetActionAnimation("Turn_L135_01", true);
        }
        else if (viewableAngle > 145 && viewableAngle <= 180)
        {
            aICharacter.characterAnimationManager.PlayTargetActionAnimation("Turn_R180_01", true);
        }
        else if (viewableAngle < -145 && viewableAngle > -180)
        {
            aICharacter.characterAnimationManager.PlayTargetActionAnimation("Turn_L180_01", true);
        }
    }

    public void RotateTowardsAgent(EnemyCharacterManager aiCharacter)
    {
        if (aiCharacter.enemyMovementManager.isMoving.GetBool())
        {
            aiCharacter.transform.rotation = aiCharacter.navMeshAgent.transform.rotation;
        }
    }

    public void RotateTowardsTargetWhileAttacking(EnemyCharacterManager aiCharacter, EnemyAttackAction currentAttack)
    {
        if (currentTarget == null) return;

        //check if we can rotate 
        if (!aiCharacter.canRotate) return;

        //rotate towards the target at a specified rotation speed during specific frames
        Vector3 targetDirection = currentTarget.transform.position - transform.position;
        targetDirection.y = 0;
        targetDirection.Normalize();

        if (targetDirection == Vector3.zero)
            targetDirection = aiCharacter.transform.forward;

        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
        //aiCharacter.transform.rotation = Quaternion.Slerp(aiCharacter.transform.rotation, targetRotation, attackTrackingSpeed);
        aiCharacter.transform.rotation = Quaternion.RotateTowards(
            aiCharacter.transform.rotation, targetRotation, currentAttack.attackTrackingSpeed * Time.deltaTime);
    }

    public void HandleActionRecovery(EnemyCharacterManager aICharacter)
    {
        if (actionRecoveryTimer > 0)
        {
            if (!aICharacter.isPerformingAction)
            {
                actionRecoveryTimer -= Time.deltaTime;
            }
        }
    }

    public bool HasRangedAttack()
    {
        for (int i = 0; i < availableAttacks.Count; i++)
        {
            if (availableAttacks[i] != null && availableAttacks[i].enemyAttackType == EnemyAttackType.Ranged)
            {
                return true;
            }
        }
        return false;
    }

    public bool HasRangedAttackAvailable(Vector3 origin, Vector3 targetPos)
    {
        for (int i = 0; i < availableAttacks.Count; i++)
        {
            var atk = availableAttacks[i];
            if (atk == null) continue;
            if (atk.enemyAttackType != EnemyAttackType.Ranged) continue;
            if (!enemy.enemyCombatManager.AttackOffCooldown(atk)) continue;

            float dist = Vector3.Distance(origin, targetPos);
            if (dist < atk.minAttackDistance || dist > atk.maxAttackDistance) continue;

            // check LOS if needed
            if (atk.requiresLOS)
            {
                if (Physics.Linecast(origin, targetPos, WorldUtilityManager.instance.GetEnviroLayers()))
                    continue;
            }
            //if makes it down to here before exiting loop, has a tranged attack available
            
            return true;
        }
        //no ranged attacks available
        //Debug.Log(false);
        return false;
    }

    public void StartCooldown(EnemyAttackAction action)
    {
        if (!action) return;
        cooldownTracker[action] = action.cooldownTime;
    }
    public void HandleCooldowns(EnemyCharacterManager enemy)
    {
        foreach (var action in availableAttacks)
        {
            if (!action) continue;
            if (!cooldownTracker.ContainsKey(action)) continue;

            cooldownTracker[action] -= Time.deltaTime;
            if(cooldownTracker[action] < 0)
            {
                cooldownTracker[action] = 0;
            }
        }
    }
    public bool AttackOffCooldown(EnemyAttackAction action)
    {
        if (cooldownTracker[action] == 0) return true;
        else return false;
    }
}
