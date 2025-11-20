using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class InputManager : MonoBehaviour
{
    public static InputManager instance;
    PlayerControls playerControls;
    public PlayerManager player;

    [Header("Allowed Inputs")]
    [SerializeField] bool allowJump;
    [SerializeField] bool allowSprint;
    [SerializeField] bool allowLockOn;
    [SerializeField] bool allowDodge;
    [SerializeField] bool allowWeaponSwap;
    [SerializeField] bool allowChargedHeavyAttacks;

    [Header("Player Movement")]
    public Vector2 movement_Input;
    public float horizontal_Input;
    public float vertical_Input;
    public float moveAmount;

    [Header("Camera Inputs")]
    [SerializeField] Vector2 camera_Input;
    public float camVerticalInput;
    public float camHorizontalInput;

    [Header("Aim Inputs")]
    [SerializeField] float mousePixelThreshold = 3f;
    [SerializeField] float stickDeadzone = 0.1f;
    public bool usingMouse;
    public bool usingController;
    public Vector2 mouse_Pos;
    private Vector2 lastMousePos;

    [Header("Attack (R1/R2) Inputs")]
    [SerializeField] bool lightAttck_Input = false;
    [SerializeField] bool heavyAttack_Input = false;
    [SerializeField] bool charge_HA_Input = false;

    [Header("Player Action Inputs")]
    [SerializeField] bool dodge_Input = false;
    [SerializeField] bool sprint_Input = false;
    [SerializeField] bool jump_Input = false;

    [Header("Lock On")]
    [SerializeField] bool lockOn_Input = false;
    [SerializeField] bool lockOn_Left_Input = false;
    [SerializeField] bool lockOn_Right_Input = false;
    private Coroutine lockOnCoroutine;

    [Header("Quick Slot Inputs")]
    [SerializeField] bool switch_Right_Weapon_Input = false;
    [SerializeField] bool switch_Left_Weapon_Input = false;

    [Header("Queued Inputs")]
    [SerializeField] float que_Input_Timer = 0;
    [SerializeField] float default_Que_Input_Timer = 0.25f;
    [SerializeField] bool input_Que_Active = false;
    [SerializeField] bool qued_LA_input = false;
    [SerializeField] bool qued_HA_input = false;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void OnSceneChange(Scene oldScene, Scene newScene)
    {
        Debug.Log(newScene.buildIndex);
        //when moving from menu scene to main gameplay scene, enable controls
        if (newScene.buildIndex == 1)
        {
            instance.enabled = true;
            if (playerControls != null)
            {
                playerControls.Enable();
            }
        }
        else
        {
            instance.enabled = false;
            if (playerControls != null)
            {
                playerControls.Disable();
            }
        }
    }
    private void OnEnable()
    {
        if (playerControls == null)
        {
            playerControls = new PlayerControls();

            playerControls.PlayerMovement.Move.performed += i => movement_Input = i.ReadValue<Vector2>();
            playerControls.PlayerMovement.Look.performed += i => camera_Input = i.ReadValue<Vector2>();

            //Aim Inputs
            playerControls.PlayerMovement.MouseAim.performed += i => mouse_Pos = i.ReadValue<Vector2>();

            //Movement Actions
            playerControls.PlayerMovement.Dodge.performed += i => dodge_Input = true;
            playerControls.PlayerMovement.Jump.performed += i => jump_Input = true;

            //Sprint Holds
            playerControls.PlayerMovement.Sprint.performed += i => sprint_Input = true;
            playerControls.PlayerMovement.Sprint.canceled += i => sprint_Input = false;

            //R1 Actions
            playerControls.PlayerActions.LightAttack.performed += i => lightAttck_Input = true;

            //R2 Actions
            playerControls.PlayerActions.HeavyAttack.performed += i => heavyAttack_Input = true;
            playerControls.PlayerActions.ChargedHeavyAttack.performed += i => charge_HA_Input = true;
            playerControls.PlayerActions.ChargedHeavyAttack.canceled += i => charge_HA_Input = false;

            //Lock on Inputs
            playerControls.PlayerActions.LockOn.performed += i => lockOn_Input = true;
            playerControls.PlayerActions.SeekLockOnTargetLeft.performed += i => lockOn_Left_Input = true;
            playerControls.PlayerActions.SeekLockOnTargetRight.performed += i => lockOn_Right_Input = true;

            //Change Equipment/Items
            playerControls.PlayerActions.Next.performed += i => switch_Right_Weapon_Input = true;
            playerControls.PlayerActions.Previous.performed += i => switch_Left_Weapon_Input = true;

            //Qued Inputs
            playerControls.PlayerActions.QuedLightAttack.performed += i => QuedInput(ref qued_LA_input);
            playerControls.PlayerActions.QuedHeavyAttack.performed += i => QuedInput(ref qued_HA_input);

            InputSystem.onActionChange += InputActionChangeCallback;
        }
    }
    private void OnApplicationFocus(bool focus)
    {
        if (enabled)
        {
            if (focus)
            {
                playerControls.Enable();
            }
            else
            {
                playerControls.Disable();
            }
        }
    }
    private void OnDestroy()
    {
        SceneManager.activeSceneChanged -= OnSceneChange;
        InputSystem.onActionChange -= InputActionChangeCallback;
    }
    private void Start()
    {
        //DontDestroyOnLoad(gameObject);
        SceneManager.activeSceneChanged += OnSceneChange;
        //instance.enabled = false;

        if (playerControls != null)
        {
            playerControls.Enable();
            //playerControls.Disable();
        }
    }
    private void Update()
    {
        HandleAllInputs();
    }
    private void InputActionChangeCallback(object obj, InputActionChange change)
    {
        if (change != InputActionChange.ActionPerformed) return;
        if (obj is not InputAction actionCalled) return;
        if (actionCalled.activeControl == null) return;


        if (change == InputActionChange.ActionPerformed)
        {
            if (obj != null && obj is InputAction action)
            { // Modern C# is usable because we're checking the type, not for null
                if (action.activeControl == null) return; // Can't use modern C# here because Destroy exists and does weird things with the memory behind the scenes
                InputDevice lastDevice = action.activeControl.device;

                //print(lastDevice.name);

                if (lastDevice is Keyboard)
                {
                    usingMouse = true;
                    usingController = false;
                }
                else if(lastDevice is Mouse)
                {
                    if (action.activeValueType != typeof(Vector2)) return;

                    Vector2 pos = action.ReadValue<Vector2>();
                    Vector2 delta = pos - lastMousePos;
                    lastMousePos = pos;

                    if (delta.sqrMagnitude >= mousePixelThreshold * mousePixelThreshold)
                    {
                        usingMouse = true;
                        usingController = false;
                    }
                }
                else
                {
                    if (action.activeValueType != typeof(Vector2)) return;

                    Vector2 v = action.ReadValue<Vector2>();
                    if (v.magnitude >= stickDeadzone)
                    {
                        usingMouse = false;
                        usingController = true;
                    }
                        
                }
            }
        }
    }
    private void HandleAllInputs()
    {
        HandleMovementInput();
        HandleCameraMovementInput();

        HandleLightAttackInput();
        HandleHeavyAttackInput();
        HandleChargeHeavyInput();

        HandleDodgeInput();
        HandleSprintInput();
        HandleJumpInput();

        HandleLockOnInput();
        HandleLockOnSwitchTargetInput();

        HandleSwitchRightWeaponSlot();
        HandleSwitchLeftWeaponSlot();

        HandleQuedInputs();
    }
    //Movement Inputs
    private void HandleMovementInput()
    {
        vertical_Input = movement_Input.y;
        horizontal_Input = movement_Input.x;

        moveAmount = Mathf.Clamp01(Mathf.Abs(vertical_Input) + Mathf.Abs(horizontal_Input));

        if (moveAmount <= 0.5 && moveAmount > 0)
        {
            moveAmount = 0.5f;
        }
        else if (moveAmount > 0.5 && moveAmount <= 1)
        {
            moveAmount = 1.0f;
        }

        if (player == null) return;

        if (moveAmount != 0)
        {
            player.playerMovementManager.isMoving.SetBool(true);
        }
        else
        {
            player.playerMovementManager.isMoving.SetBool(false);
        }

        if (player.playerCombatManager.isLockedOn || player.playerMovementManager.isSprinting)
        {
            player.playerAnimatorManager.UpdateAnimatorMovementParameters(horizontal_Input, vertical_Input, player.playerMovementManager.isSprinting);
        }
        else
        {
            //only pass vertical since strafing is only while locked on, want to only run forward with camera movement right now 
            player.playerAnimatorManager.UpdateAnimatorMovementParameters(0, moveAmount, player.playerMovementManager.isSprinting);
        }

    }
    private void HandleCameraMovementInput()
    {
        camVerticalInput = camera_Input.y;
        camHorizontalInput = camera_Input.x;
    }

    //Attack Inputs
    private void HandleLightAttackInput()
    {
        if (lightAttck_Input)
        {
            lightAttck_Input = false;
            //do nothing if UI window is open 

            player.playerCombatManager.SetCharacterActionHand(true);

            //Debug.Log("Light Attack Input");

            player.playerCombatManager.PerformWeaponBasedAction(
                player.playerInventoryManager.currentRightHandWeapon.oh_r1_Action,
                player.playerInventoryManager.currentRightHandWeapon);

        }
    }
    private void HandleHeavyAttackInput()
    {
        if (heavyAttack_Input)
        {
            heavyAttack_Input = false;
            //do nothing if UI window is open 

            player.playerCombatManager.SetCharacterActionHand(true);

            //if we are using 2 hands, do the 2hand action instead of one hand action 
            player.playerCombatManager.PerformWeaponBasedAction(
                player.playerInventoryManager.currentRightHandWeapon.oh_r2_Action,
                player.playerInventoryManager.currentRightHandWeapon);
        }
    }
    private void HandleChargeHeavyInput()
    {
        if (!allowChargedHeavyAttacks) return;

        if (player.isPerformingAction)
        {
            //only want to check this if we are performing an action already that requires charging 
            if (player.playerCombatManager.isUsingRightHand.GetBool())
            {
                player.playerCombatManager.isChargingAttack.SetBool(charge_HA_Input);
            }
        }
    }

    //Player Actions
    private void HandleDodgeInput()
    {
        if (!allowDodge) return;
        if (dodge_Input)
        {
            dodge_Input = false;
            //disable when menu is open
            //check for stamina
            player.playerMovementManager.AttemptToDodge();
        }
    }
    private void HandleSprintInput()
    {
        if (!allowSprint) return;
        if (sprint_Input)
        {
            player.playerMovementManager.HandleSprinting();
        }
        else
        {
            player.playerMovementManager.isSprinting = false;
        }
    }
    public void HandleJumpInput()
    {
        if (!allowJump) return;

        if (jump_Input)
        {
            jump_Input = false;

            //dont do this if a menu is open

            //check for grounded status or performing other actions

            player.playerMovementManager.AttemptToJump();
        }
    }
    private void HandleSwitchRightWeaponSlot()
    {
        if (!allowWeaponSwap) return;
        if (switch_Right_Weapon_Input)
        {
            switch_Right_Weapon_Input = false;
            player.playerEquipmentManager.SwitchRightWeapon();
        }
    }
    private void HandleSwitchLeftWeaponSlot()
    {
        if (!allowWeaponSwap) return;
        if (switch_Left_Weapon_Input)
        {
            switch_Left_Weapon_Input = false;
            player.playerEquipmentManager.SwitchLeftWeapon();
        }
    }

    //Lock On Inputs
    private void HandleLockOnInput()
    {
        if (!allowLockOn) return;
        //check for dead target
        if (player.playerCombatManager.isLockedOn)
        {
            if (player.playerCombatManager.currentTarget == null) return;

            if (player.isDead)
            {
                player.playerCombatManager.isLockedOn = false;
                if (lockOnCoroutine != null)
                {
                    Debug.Log("Dead target, select new one");
                    StopCoroutine(lockOnCoroutine);
                }
                lockOnCoroutine = StartCoroutine(PlayerCamera.instance.WaitThenFindNewTarget());
            }
            //try to find new target to lock onto, makes sure the coroutine can not be running more than one at a time 
        }

        //if already locked on, unlock on from targets
        if (lockOn_Input && player.playerCombatManager.isLockedOn)
        {
            lockOn_Input = false;
            PlayerCamera.instance.ClearLockOnTargets();
            player.playerCombatManager.isLockedOn = false;


            return;
        }
        if (lockOn_Input && !player.playerCombatManager.isLockedOn)
        {
            lockOn_Input = false;

            //how does lock on affect ranged weapons?

            PlayerCamera.instance.HandleLocatingLockOnTargets();

            if (PlayerCamera.instance.nearestLockOnTarget != null)
            {
                //set the target as our current lock on target 
                player.playerCombatManager.SetTarget(PlayerCamera.instance.nearestLockOnTarget);
                player.playerCombatManager.isLockedOn = true;
            }
        }
    }
    private void HandleLockOnSwitchTargetInput()
    {
        if (lockOn_Left_Input)
        {
            lockOn_Left_Input = false;
            if (player.playerCombatManager.isLockedOn) //check if already locked on something
            {
                PlayerCamera.instance.HandleLocatingLockOnTargets();
                if (PlayerCamera.instance.leftLockOnTarget != null)
                {
                    player.playerCombatManager.SetTarget(PlayerCamera.instance.leftLockOnTarget);
                }
            }
        }
        if (lockOn_Right_Input)
        {
            lockOn_Right_Input = false;
            if (player.playerCombatManager.isLockedOn) //check if already locked on something
            {
                PlayerCamera.instance.HandleLocatingLockOnTargets();
                if (PlayerCamera.instance.rightLockOnTarget != null)
                {
                    player.playerCombatManager.SetTarget(PlayerCamera.instance.rightLockOnTarget);
                }
            }
        }
    }

    //Input Queing
    private void QuedInput(ref bool quedInput) //passing a ref means we pass a specific bool, and not the value of the bool(True or false)
    {
        //reset all qued inputs, only one at a time allowed
        qued_LA_input = false;
        qued_HA_input = false;
        //qued_l1_input = false;
        //qued_l2_input = false;

        //check for UI window being open, return if so 

        if (player.isPerformingAction || player.playerMovementManager.isJumping)
        {
            quedInput = true;
            que_Input_Timer = default_Que_Input_Timer;
            input_Que_Active = true;
        }
    }
    private void ProcessQuedInputs()
    {
        if (player.isDead) return;

        if (qued_LA_input) lightAttck_Input = true;
        if (qued_HA_input) heavyAttack_Input = true;
    }
    private void HandleQuedInputs()
    {
        if (input_Que_Active)
        {
            //while timer is > 0, keep attempting the input press
            if (que_Input_Timer > 0)
            {
                que_Input_Timer -= Time.deltaTime;
                ProcessQuedInputs();
            }
            else
            {
                qued_LA_input = false;
                qued_HA_input = false;
                //qued_l1_input = false;
                //qued_l2_input = false;
                input_Que_Active = false;
                que_Input_Timer = 0;
            }
        }
    }


}
