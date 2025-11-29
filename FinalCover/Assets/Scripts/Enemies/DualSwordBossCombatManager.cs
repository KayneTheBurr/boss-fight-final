using System.Collections.Generic;
using UnityEngine;

public class DualSwordBossCombatManager : EnemyCombatManager
{
    [Header("Damage Colliders")]
    [SerializeField] WolfDamageCollider teethDamageCollider;
    [SerializeField] WolfDamageCollider rightClawDamageCollider;
    [SerializeField] WolfDamageCollider leftClawDamageCollider;

    [Header("Damage")]
    [SerializeField] public float physicalDamage = 15;
    [SerializeField] public float iceDamage = 10;
    [SerializeField] public float lungeBiteAttack_01_DamageModifier = 1f;
    [SerializeField] public float sideBiteAttack_01_DamageModifier = 1.2f;
    [SerializeField] public float swipeAttack_01_DamageModifier = 1f;
    [SerializeField] public float swipeAttack_02_DamageModifier = 1.5f;

    [Header("Wolf Turn Values")]
    [SerializeField] string turnLeftState = "Wolf_Combat_Turn_L";
    [SerializeField] string turnRightState = "Wolf_Combat_Turn_R";
    [SerializeField] string quickTurnState = "Wolf_QuickTurn_Slash";
    [SerializeField] AnimationClip turn90Clip;    // 90 degree turn anim
    [SerializeField] float desired90Time = 0.35f; // speed of turning 90 deg
    [SerializeField] float turnThreshold = 40f;   // start in-place turn if abs(angle) >= this
    [SerializeField] float quickTurnThreshold = 150f;
    [SerializeField] float quickTurnRange = 2.5f;
    [SerializeField] float quickTurnCooldown = 3f;

    [Header("Quickturn Slash Settings")]
    [SerializeField] GameObject quickTurnSlashEffect;
    [SerializeField] float slashHeight = 2f;
    [SerializeField] float slashWidth;
    [SerializeField] float slashRange;
    [SerializeField] float vfxForwardOffset = 1f;
    [SerializeField] float quickTurnSlashModifier = 1f;

    [Header("Arena Settings")]
    [SerializeField] private float arenaRadius = 16f;
    public Transform arenaCenter;
    [SerializeField] private float spawnRayHeight = 8f;

    [Header("Icicle Ability Settings")]
    public GameObject iciclePrefab;
    public bool _spawnIcicles = false;
    public List<GameObject> iciclesToSpawn;
    public float icicleSpawnRadius;
    [SerializeField] private int totalSpikes = 30;
    [SerializeField] private float icicleTelegraphDuration = 2.0f;  //how long BEFORE ice ground starts spawning
    [SerializeField] private float spreadDuration = 2.75f;     // how long the ice spots take to fulls spawn in 
    [SerializeField] private float fadeLastT = 0.25f;
    [SerializeField] private float eruptionSweep = 0.75f;     // time from first eruption to last 
    [SerializeField] private float hitWindow = 0.10f;         // collider ON duration per spike
    [SerializeField] private float ringRadius = 12f;
    [SerializeField] private float minSeparation = 1.4f;      // distance between each vfx object 
    [SerializeField] private float despawnAfter = 2.5f;

    [Header("Ice Beams Settings")]
    public GameObject iceBeamPrefab;
    [SerializeField] private int totalBeams = 8;
    public List<GameObject> iceBeamsToSpawn;
    public float iceBeamSpawnRadius;
    [SerializeField] float iceBeamHitTime = 0.75f;
    [SerializeField] float iceBeamWaveTime = 0.25f;
    [SerializeField] int numberOfWaves = 4;
    public float iceBeamDeswapwnTime = .5f;


     protected override void Start()
    {
        base.Start();
        SetDamageColliders();
    }
    public override void SetDamageColliders()
    {
        base.SetDamageColliders();

    }

    //Pivot and Quickturn
    public override void PivotTowardsTarget(EnemyCharacterManager wolfChar)
    {
        //dont include base, want to do different things

        if (currentTarget == null) return;
        if (wolfChar.isPerformingAction) return;

        Vector3 dir = currentTarget.transform.position - wolfChar.transform.position;
        float angle = WorldUtilityManager.instance.GetAngleOfTarget(wolfChar.transform, dir);
        float absA = Mathf.Abs(angle);

        // Quick turn when the player is close enough and right behind them 
        if (absA >= quickTurnThreshold && distanceFromTarget <= quickTurnRange) //&& Time.time >= _nextQuickTurn
        {
            Debug.Log("Do a quickturn?");
            wolfChar.characterAnimationManager.PlayTargetActionAnimation(quickTurnState, true);

            wolfChar.isPerformingAction = true;
            wolfChar.canRotate = false;
            actionRecoveryTimer = 0.5f;
            return;
        }

        // 2) Regular in-place turn (pick side by sign)
        if (absA >= turnThreshold)
        {
            string state = angle >= 0f ? turnRightState : turnLeftState;
            wolfChar.animator.speed = SpeedForTurn(Mathf.Min(absA, 90f));
            wolfChar.characterAnimationManager.PlayTargetActionAnimation(state, true);

            wolfChar.isPerformingAction = true;
            wolfChar.canRotate = false;
            actionRecoveryTimer = desired90Time * 0.5f;
        }
    }
    public float SpeedForTurn(float angleDeg)
    {
        if (!turn90Clip) return 1f;
        float targetTime = desired90Time * (angleDeg / 90f);
        float baseLen = Mathf.Max(turn90Clip.length, 0.001f);
        return Mathf.Clamp(baseLen / targetTime, 0.5f, 3f);
    }
}
