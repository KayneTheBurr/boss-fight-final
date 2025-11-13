using System.Collections;
using System.Globalization;
using TMPro;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyCharacterManager : CharacterManager
{
    [HideInInspector] public EnemyCombatManager enemyCombatManager;
    [HideInInspector] public EnemyAnimationManager enemyAnimationManager;
    [HideInInspector] public EnemyMovementManager enemyMovementManager;
    [HideInInspector] public EnemyStatsManager enemyStatsManager;


    [Header("NavMesh Agent")]
    public NavMeshAgent navMeshAgent;
    public Vector3 myTargetDestination;

    [Header("Current State")]
    [SerializeField] AIStates currentState;
    public bool inCombat;

    [Header("States")]
    public IdleState idle;
    public PursueTargetState pursueTarget;
    public CombatStanceState combatStance;
    public AttackState attack;

    [Header("Boss Enemy HUD")]
    [SerializeField] public bool isBoss = false;
    [SerializeField] TMP_Text bossNameLabel;
    [SerializeField] TMP_Text bossNameLabelShadow;
    public GameObject bossPanel;
    public Slider bossHPBar;
    private bool _bossHudBound = false;


    protected override void Awake()
    {
        base.Awake();

        navMeshAgent = GetComponentInChildren<NavMeshAgent>();

        enemyCombatManager = GetComponent<EnemyCombatManager>();
        enemyAnimationManager = GetComponent<EnemyAnimationManager>();
        enemyMovementManager = GetComponent<EnemyMovementManager>();
        enemyStatsManager = GetComponent<EnemyStatsManager>();

        

        idle = Instantiate(idle);
        pursueTarget = Instantiate(pursueTarget);
        combatStance = Instantiate(combatStance);
        attack = Instantiate(attack);

        currentState = idle;
    }
    protected override void Start()
    {
        base.Start();
        navMeshAgent.SetDestination(transform.position);

        bossPanel = PlayerUIManager.instance.playerHUDManager.bossPanel;
        bossNameLabel = PlayerUIManager.instance.playerHUDManager.bossNameLabel;
        bossHPBar = PlayerUIManager.instance.playerHUDManager.bossHPBar;
        bossNameLabelShadow = PlayerUIManager.instance.playerHUDManager.bossNameLabelShadow;

    }
    protected override void OnEnable()
    {
        base.OnEnable();
    }
    protected override void OnDisable()
    {
        base.OnDisable();
    }
    private void ProcessStateMachine()
    {
        //check if current state is NOT null, and if so, run the TICK function. THis is called in fixed update. 
        AIStates nextState = currentState?.Tick(this);

        if (nextState != null)
        {
            currentState = nextState;
        }

        //reset the position/rotation after teh state machine has processed it
        navMeshAgent.transform.localPosition = Vector3.zero;
        navMeshAgent.transform.localRotation = Quaternion.identity;

        if (enemyCombatManager.currentTarget != null)
        {
            enemyCombatManager.distanceFromTarget = Vector3.Distance(
                enemyCombatManager.currentTarget.transform.position, transform.position);

            enemyCombatManager.targetDirection = enemyCombatManager.currentTarget.transform.position - transform.position;

            enemyCombatManager.viewableAngle = WorldUtilityManager.instance.GetAngleOfTarget(
                transform, enemyCombatManager.targetDirection);
        }


        if (navMeshAgent.enabled)
        {
            Vector3 agentDestination = navMeshAgent.destination;
            myTargetDestination = agentDestination;
            float remainingDistance = Vector3.Distance(agentDestination, transform.position);

            if (remainingDistance > navMeshAgent.stoppingDistance)
            {
                //Debug.Log("WE are moving!");
                enemyMovementManager.isMoving.SetBool(true);
            }
            else
            {
                enemyMovementManager.isMoving.SetBool(false);
            }
        }
        else
        {
            enemyMovementManager.isMoving.SetBool(false);
        }
    }
    protected override void Update()
    {
        base.Update();
        enemyCombatManager.HandleActionRecovery(this);
        enemyCombatManager.HandleCooldowns(this);
    }
    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        ProcessStateMachine();
    }

    public void HandlePlayerHUDBossUI(bool inCombat)
    {
        if (!isBoss) return;

        if (inCombat)
            EnableBossHUD();
        else
            DisableBossHUD();
        
    }
    private void EnableBossHUD()
    {
        if (!isBoss || _bossHudBound) return;
        
        //set initial health
        bossHPBar.maxValue = enemyStatsManager.maxHealth.GetInt();
        bossHPBar.value = enemyStatsManager.currentHealth.GetFloat();

        //set name
        bossNameLabelShadow.text = characterName.GetString();
        bossNameLabel.text = characterName.GetString();

        //toggle listener and bool check
        enemyStatsManager.currentHealth.OnFloatChanged += enemyStatsManager.OnBossHPChanged;
        _bossHudBound = true;

        bossPanel.SetActive(true);
    }
    public void DisableBossHUD()
    {
        if (!_bossHudBound) return;

        enemyStatsManager.currentHealth.OnFloatChanged -= enemyStatsManager.OnBossHPChanged;
        _bossHudBound = false;

        bossPanel.SetActive(false);
    }
    public override IEnumerator HandleDeathEvents(bool manuallySelectDeathAnim = false)
    {
        if (isBoss) DisableBossHUD();
        return base.HandleDeathEvents(manuallySelectDeathAnim);
    }
}
