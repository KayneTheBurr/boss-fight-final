using System.Collections.Generic;
using UnityEngine;

public class DualSwordBossCombatManager : AssassinEnemyCombatManager
{
    [Header("Phase")]
    public BossPhase currentPhase = BossPhase.Phase1;

    [Header("Damage Colliders")]
    [SerializeField] MeleeWeaponDamageCollider rightSwordCollider;
    [SerializeField] MeleeWeaponDamageCollider leftSwordCollider;

    [Header("Damage")]
    [SerializeField] public float physicalDamage = 15;
    [SerializeField] public float lightAttack_01_DamageModifier = 1f;
    [SerializeField] public float lightAttack_02_DamageModifier = 1f;
    [SerializeField] public float lightAttack_03_DamageModifier = 2f;
    [SerializeField] public float drillDash_DamageModifier = 1.5f;
    [SerializeField] public float aoeSpinSlash_DamageModifier = 1.5f;

    [Header("Dodge Tuning")]
    [SerializeField] float backDodgeAngle = 10f;
    [SerializeField] float frontDodgeDistance = 5f;
    [SerializeField] AnimationCurve frontDodgeCurve;
    [SerializeField] float backDodgeDistance = 5f;
    [SerializeField] AnimationCurve backDodgeCurve;
    [SerializeField] float leftDodgeDistance = 5f;
    [SerializeField] AnimationCurve leftDodgeCurve;
    [SerializeField] float rightDodgeDistance = 5f;
    [SerializeField] AnimationCurve rightDodgeCurve;

    [Header("Arena Settings")]
    [SerializeField] private float arenaRadius = 16f;
    public Transform arenaCenter;

    protected override void Start()
    {
        base.Start();
        
    }


    //Collider Management
    public override void SetDamageColliders()
    {
        base.SetDamageColliders();
        SetLeftSwordCollider();
        SetRightSwordCollider();
    }
    public void SetLeftSwordCollider()
    {
        leftSwordCollider.physicalDamage = physicalDamage;
    }
    public void SetRightSwordCollider()
    {
        rightSwordCollider.physicalDamage = physicalDamage;
    }
    public void OpenLeftSwordCollider()
    {
        leftSwordCollider.EnableDamageCollider();
    }
    public void CloseLeftSwordCollider()
    {
        leftSwordCollider.DisableDamageCollider();
    }
    public void OpenRightSwordCollider()
    {
        rightSwordCollider.EnableDamageCollider();
    }
    public void CloseRightSwordCollider()
    {
        rightSwordCollider.DisableDamageCollider();
    }


    //Pivot and Dodge
    public override void PivotTowardsTarget(EnemyCharacterManager enemy)
    {
        if (enemy.isPerformingAction) return;
        
        if (Mathf.Abs(viewableAngle) < 70) return;

        Debug.Log($"Turn bc my angle is {viewableAngle}!");

        if (enemy.enemyMovementManager.isMoving.GetBool())
        {
            if(viewableAngle >= 70 && viewableAngle <= 120)
            {
                enemy.characterAnimationManager.PlayTargetActionAnimation("Jog_Turn_R90", true);
            }
            else if (viewableAngle <= -70 && viewableAngle >= -120)
            {
                enemy.characterAnimationManager.PlayTargetActionAnimation("Jog_Turn_L90", true);
            }
            else if( viewableAngle > 120 && viewableAngle <= 180)
            {
                enemy.characterAnimationManager.PlayTargetActionAnimation("Jog_Turn_180", true);
            }
            else if (viewableAngle < -120 && viewableAngle > -180)
            {
                enemy.characterAnimationManager.PlayTargetActionAnimation("Jog_Turn_180", true);
            }

        }
        else if(!enemy.enemyMovementManager.isMoving.GetBool())
        {
            if (viewableAngle >= 70 && viewableAngle <= 120)
            {
                enemy.characterAnimationManager.PlayTargetActionAnimation("Idle_Turn_R90", true);
            }
            else if (viewableAngle <= -70 && viewableAngle >= -120)
            {
                enemy.characterAnimationManager.PlayTargetActionAnimation("Idle_Turn_L90", true);
            }
            else if (viewableAngle > 120 && viewableAngle <= 180)
            {
                enemy.characterAnimationManager.PlayTargetActionAnimation("Idle_Turn_180", true);
            }
            else if (viewableAngle < -120 && viewableAngle > -180)
            {
                enemy.characterAnimationManager.PlayTargetActionAnimation("Idle_Turn_180", true);
            }
        }
    }
    public override void PerformDodge(EnemyCharacterManager enemy)
    {
        base.PerformDodge(enemy);
        if (enemy.isPerformingAction) return;

        float angle = viewableAngle;
        string dodgeAnim;

        if (Mathf.Abs(angle) < backDodgeAngle)
        {
            dodgeAnim = "Back_Dodge_01";
            enemy.enemyMovementManager.SetManualMotionValuesRemotely(
                backDodgeDistance, 1, ManualMotionDirection.LocalBackward, backDodgeCurve);
        }
        // Player more on the right side -> dodge left (from her POV, away from player’s weapon)
        else if (angle > 0)
        {
            dodgeAnim = "Left_Dodge_01";
            enemy.enemyMovementManager.SetManualMotionValuesRemotely(
                leftDodgeDistance, 1, ManualMotionDirection.LocalLeft, leftDodgeCurve);
        }
        // Player more on the left side -> dodge right
        else
        {
            dodgeAnim = "Right_Dodge_01";
            enemy.enemyMovementManager.SetManualMotionValuesRemotely(
                rightDodgeDistance, 1, ManualMotionDirection.LocalRight, rightDodgeCurve);
        }

        // check distanceFromTarget to avoid dodging when too far?

        enemy.characterAnimationManager.PlayTargetActionAnimation(dodgeAnim, false);
        enemy.isPerformingAction = true;

        enemy.enemyMovementManager.StartManualMotion();

        //stop the agent path while dodging:
        enemy.navMeshAgent.ResetPath();
    }


    //Projectile and Gravity Attacks
    public void GravityProjectileAttack()
    {

    }
}
