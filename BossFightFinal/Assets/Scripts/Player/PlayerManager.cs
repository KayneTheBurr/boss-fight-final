using UnityEngine;

public class PlayerManager : CharacterManager
{
    [HideInInspector] public PlayerMovementManager playerMovementManager;
    [HideInInspector] public PlayerCombatManager playerCombatManager;
    [HideInInspector] public PlayerAnimationManager playerAnimatorManager;
    [HideInInspector] public PlayerStatManager playerStatsManager;
    [HideInInspector] public PlayerInventoryManager playerInventoryManager;
    [HideInInspector] public PlayerEquipmentManager playerEquipmentManager;

    protected override void Awake()
    {
        //do character manager awake stuff 
        base.Awake();

        //after doing charcter stuff then player only stuff gets done next 
        playerMovementManager = GetComponent<PlayerMovementManager>();
        playerCombatManager = GetComponent<PlayerCombatManager>();
        playerAnimatorManager = GetComponent<PlayerAnimationManager>();
        playerStatsManager = GetComponent<PlayerStatManager>();
        playerInventoryManager = GetComponent<PlayerInventoryManager>();
        playerEquipmentManager = GetComponent<PlayerEquipmentManager>();
        
    }

    protected override void Start()
    {
        base.Start();
        //DontDestroyOnLoad(gameObject);

        InputManager.instance.player = this;
        PlayerCamera.instance.player = this;
        //WorldSaveGameManager.instance.player = this;

        PlayerCamera.instance.SetPlayerAsFollowTarget();

    }

    protected override void Update()
    {
        base.Update();
        playerMovementManager.AllMovement();
        playerStatsManager.RegenerateStamina();
        playerStatsManager.RegenerateMana();
    }

}
    

