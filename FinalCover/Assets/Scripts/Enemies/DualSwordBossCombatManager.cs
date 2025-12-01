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

    [Header("Turn Values")]
    [SerializeField] string turnRightIdle = "Idle_Turn_R90";
    [SerializeField] string turnLeftIdle = "Idle_Turn_L90";
    [SerializeField] string turn180Idle = "Idle_Turn_180";
    [SerializeField] string turnRightWalk = "Walk_Turn_R90";
    [SerializeField] string turnLeftWalk = "Walk_Turn_L90";
    [SerializeField] string turn180Walk = "Walk_Turn_180";
    [SerializeField] string turnRightJog = "Jog_Turn_R90";
    [SerializeField] string turnLeftJog = "Jog_Turn_L90";
    [SerializeField] string turn180Jog = "Jog_Turn_180";

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


    //Pivot and Quickturn
    public override void PivotTowardsTarget(EnemyCharacterManager wolfChar)
    {
        
    }


    //Projectile and Gravity Attacks
    public void GravityProjectileAttack()
    {

    }
}
