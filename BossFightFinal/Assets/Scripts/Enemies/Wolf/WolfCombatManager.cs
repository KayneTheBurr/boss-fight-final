using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;
using static UnityEngine.Rendering.DebugUI.Table;

public class WolfCombatManager : EnemyCombatManager
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
        SetWolfDamageColliders();
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
    public void TriggerSlashEffect()
    {
        Debug.Log("Triggered a Quickturn Slash Event!");

        var spawnPos = transform.position + Vector3.up * slashHeight + transform.forward * vfxForwardOffset;
        var spawnRot = transform.rotation;

        if (quickTurnSlashEffect)
        {
            GameObject slashObj = Instantiate(quickTurnSlashEffect, spawnPos, spawnRot);
            StartCoroutine(DespawnAfter(slashObj, 0.3f));
        }
            
        Vector3 baseCenter = transform.position + Vector3.up * slashHeight;
        Vector3 tipCenter = baseCenter + transform.forward * slashRange;

        // Grab any potential targets in the capsule
        var hits = Physics.OverlapCapsule(
            baseCenter, tipCenter, slashWidth * 0.5f, WorldUtilityManager.instance.GetCharacterLayers(), QueryTriggerInteraction.Ignore);

        Debug.DrawRay(baseCenter, (tipCenter - baseCenter), Color.cyan, 0.25f);

        var charactersDamaged = new HashSet<CharacterManager>();

        for (int i = 0; i < hits.Length; i++)
        {
            var cm = hits[i].GetComponentInParent<CharacterManager>();
            if (cm && charactersDamaged.Add(cm) && cm != enemy)
            {
                SlashDamage(cm);
            }
        }
    }
    protected void SlashDamage(CharacterManager cm)
    {
        if (cm.characterCombatManager.isInvulnerable) return;

        Debug.Log(cm.characterName.GetString());

        TakeDamageEffect damageEffect = Instantiate(WorldCharacterEffectsManager.instance.takeDamageEffect);

        damageEffect.physicalDamage = physicalDamage * quickTurnSlashModifier;
        damageEffect.iceDamage = iceDamage * quickTurnSlashModifier;

        cm.characterEffectsManager.ProcessInstantEffects(damageEffect);
    }


    //Collider Management
    public void SetWolfDamageColliders()
    {
        teethDamageCollider.physicalDamage = physicalDamage;
        teethDamageCollider.iceDamage = iceDamage;

        rightClawDamageCollider.physicalDamage = physicalDamage;
        rightClawDamageCollider.iceDamage = iceDamage;

        leftClawDamageCollider.physicalDamage = physicalDamage;
        leftClawDamageCollider.iceDamage = iceDamage;
    }
    public void OpenTeethDamageCollider()
    {
        teethDamageCollider.EnableDamageCollider();
    }
    public void CloseTeethDamageCollider()
    {
        teethDamageCollider.DisableDamageCollider();
    }
    public void OpenRightHandDamageCollider()
    {
        rightClawDamageCollider.EnableDamageCollider();
    }
    public void CloseRightHandDamageCollider()
    {
        rightClawDamageCollider.DisableDamageCollider();
    }
    public void OpenLeftHandDamageCollider()
    {
        leftClawDamageCollider.EnableDamageCollider();
    }
    public void CloseLeftHandDamageCollider()
    {
        leftClawDamageCollider.DisableDamageCollider();
    }


    //Timed Magic Attacks
    public void StartIcicleAttack()
    {
        enemy.animator.speed = 0;
        enemy.navMeshAgent.isStopped = true;
        iciclesToSpawn.Clear();
        StartCoroutine(IcicleSpawnRoutine());
    }
    public void StartIceBeamsAttack()
    {
        enemy.animator.speed = 0;
        enemy.navMeshAgent.isStopped = true;
        iceBeamsToSpawn.Clear();
        StartCoroutine(IceBeamSpawnRoutine());
    }
    private IEnumerator IcicleSpawnRoutine()
    {
        _spawnIcicles = true;

        int N = Mathf.Max(2, totalSpikes);
        float dSpawn = spreadDuration / (N - 1);
        float dDet = eruptionSweep / (N - 1);
        float detonateStart = spreadDuration + fadeLastT;

        var spawnLocations = new List<(Vector3 pos, Quaternion rot, float dist2)>(N);
        
        var used = new HashSet<Vector2Int>();

        for (int i = 0; i < N; i++) //for each spike we want to spawn
        {
            // Pick a ground point (unique-ish cells so they don't overlap)
            Vector3 pos;
            Quaternion rot;

            for (int tries = 0; ; tries++) //try to find as place to spawn
            {
                var r = Random.Range(0f, Mathf.Max(0f, arenaRadius));
                var theta = Random.Range(0f, Mathf.PI * 2f);
                var flat = new Vector2(Mathf.Cos(theta), Mathf.Sin(theta)) * r; //pick place in 2d plane
                var start = arenaCenter.position + new Vector3(flat.x, spawnRayHeight, flat.y); //point to raycast from to get floor height and normal

                if (!Physics.Raycast(start, Vector3.down, out var hit, spawnRayHeight * 2f, WorldUtilityManager.instance.GetEnviroLayers(), QueryTriggerInteraction.Ignore))
                {
                    continue;
                }

                //check and locate the point in a grid
                var key = new Vector2Int(Mathf.RoundToInt(hit.point.x / minSeparation), Mathf.RoundToInt(hit.point.z / minSeparation));

                if (used.Add(key)) //if unused grid, add it in 
                {
                    pos = hit.point;
                    rot = Quaternion.FromToRotation(Vector3.up, hit.normal);
                    spawnLocations.Add((pos, rot, (pos - transform.position).sqrMagnitude));
                    break;
                }
                if (tries > 200) //in case no where is available dont crash unity please
                {
                    pos = start;
                    rot = Quaternion.identity;
                    spawnLocations.Add((pos, rot, (pos - transform.position).sqrMagnitude));
                    break;
                }
            }
        }

        spawnLocations.Sort((a, b) => a.dist2.CompareTo(b.dist2));

        for (int i = 0; i< spawnLocations.Count; i++)
        {
            var icicle = Instantiate(iciclePrefab, spawnLocations[i].pos, spawnLocations[i].rot);
            var vfx = icicle.GetComponentInChildren<UnityEngine.VFX.VisualEffect>();
            var col = icicle.GetComponent<Collider>(); // or damage collider mayeb
            if (col) col.enabled = false; //make sure collider off at first 

            // When THIS indicator appears and when it should erupt
            float spawnDelayInst = i * dSpawn; //spawn delay times which spike it is in series
            float detonateDelayInst = i * dDet; //detonate delay so they cascade
            float delayToEruptFromNow = (detonateStart) + detonateDelayInst - spawnDelayInst; 

            // set the spike spawn delay in vfx graph
            if (vfx)
            {
                vfx.SetFloat("SpikeSpawnDelay", delayToEruptFromNow);
            }
            // Enable the collider exactly during the hit window
            StartCoroutine(EnableColliderWindow(col, delayToEruptFromNow, hitWindow));

            // Optional cleanup
            StartCoroutine(DespawnAfter(icicle, delayToEruptFromNow + hitWindow + despawnAfter));

            iciclesToSpawn.Add(icicle);

            // Wait until it’s time to spawn the next indicator
            yield return new WaitForSeconds(dSpawn);
        }


        _spawnIcicles = false;
        enemy.animator.speed = 1;
        enemy.navMeshAgent.isStopped = false;
    }
    private IEnumerator IceBeamSpawnRoutine()
    {
        int N = Mathf.Max(2, totalBeams);
        var spawnLocations = new List<(Vector3 pos, Quaternion rot)>(N);
        int waveCount = 0;

        while (waveCount < numberOfWaves)
        {

            spawnLocations.Clear();

            for (int i = 0; i < N; i++) //for each spike we want to spawn
            {
                Vector3 pos;
                Quaternion rot;

                for (int tries = 0; ; tries++) //try to find as place to spawn
                {
                    var r = Random.Range(0f, Mathf.Max(0f, arenaRadius));
                    var theta = Random.Range(0f, Mathf.PI * 2f);
                    Vector2 flat = new Vector2(Mathf.Cos(theta), Mathf.Sin(theta)) * r; //pick place in 2d plane
                    var start = arenaCenter.position + new Vector3(flat.x, spawnRayHeight, flat.y); //point to raycast from to get floor height and normal

                    if (Physics.Raycast(start, Vector3.down, out var hit, spawnRayHeight * 2f, WorldUtilityManager.instance.GetEnviroLayers(), QueryTriggerInteraction.Ignore))
                    {
                        pos = hit.point;
                        rot = Quaternion.FromToRotation(Vector3.up, hit.normal);
                        spawnLocations.Add((pos, rot));
                        break;
                    }

                    if (tries > 200) break;
                }
            }

            for (int i = 0; i < spawnLocations.Count; i++) //spawn an ice beam at each of the locations
            {
                var iceBeam = Instantiate(iceBeamPrefab, spawnLocations[i].pos, spawnLocations[i].rot);
                var vfx = iceBeam.GetComponentInChildren<UnityEngine.VFX.VisualEffect>();
                var col = iceBeam.GetComponent<Collider>(); // or damage collider mayeb
                if (col) col.enabled = false; //make sure collider off at first 

                // Enable the collider exactly during the hit window
                StartCoroutine(EnableColliderWindow(col, iceBeamHitTime, hitWindow));

                // Optional cleanup
                StartCoroutine(DespawnAfter(iceBeam, iceBeamHitTime + hitWindow + iceBeamDeswapwnTime));

                iceBeamsToSpawn.Add(iceBeam);
                yield return new WaitForSeconds(0.1f);
            }
            
            waveCount++;
            yield return new WaitForSeconds(iceBeamWaveTime);
        }

        enemy.animator.speed = 1;
        enemy.navMeshAgent.isStopped = false;
    }
    private IEnumerator EnableColliderWindow(Collider c, float delay, float window)
    {
        if (!c) yield break;
        yield return new WaitForSeconds(delay);
        c.enabled = true;
        yield return new WaitForSeconds(window);
        if (c) c.enabled = false;
    }
    private IEnumerator DespawnAfter(GameObject g, float t)
    {
        yield return new WaitForSeconds(t);
        if (g) Destroy(g);
    }
}
