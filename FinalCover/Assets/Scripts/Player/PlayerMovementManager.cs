using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovementManager : CharacterMovementManager
{
    PlayerManager player;

    [SerializeField] public float verticalMovement, horizontalMovement, moveAmount;

    [Header("Movement Settings")]
    private Vector3 moveDirection;
    private Vector3 targetRotateDirection;
    [SerializeField] float rotationSpeed;
    [SerializeField] float runSpeed;
    [SerializeField] float sprintSpeed;
    [SerializeField] public float walkSpeed;
    public float sprintStaminaCost;

    [Header("Jump")]
    [SerializeField] float jumpStaminaCost = 15;
    [SerializeField] float jumpHeight = 2;
    private Vector3 jumpDirection;
    [SerializeField] float jumpForwardSpeed = 5;
    [SerializeField] float airborneManeuverSpeed = 3;

    [Header("Dodge")]
    private Vector3 dodgeDirection;
    [SerializeField] float dodgeStaminaCost = 0;
    [SerializeField] float backStepStaminaCost = 0;


    protected override void Awake()
    {
        base.Awake();
        player = GetComponent<PlayerManager>();
    }
    protected override void OnEnable()
    {
        base.OnEnable();
    }
    protected override void OnDisable()
    {
        base.OnDisable();
    }
    protected override void Update()
    {
        base.Update();
        if (player)
        {
            player.verticalMovement = verticalMovement;
            player.horizontalMovement = horizontalMovement;
            player.moveAmount = moveAmount;


            if (player.playerCombatManager.isLockedOn || isSprinting)
            {
                player.playerAnimatorManager.UpdateAnimatorMovementParameters(horizontalMovement, verticalMovement, isSprinting);
            }
            else
            {
                //only pass vertical since strafing is only while locked on, want to only run forward with camera movement right now 
                player.playerAnimatorManager.UpdateAnimatorMovementParameters(0, moveAmount, isSprinting);
            }
        }
        
    }
    public void AllMovement()
    {
        HandleGroundedMovement();
        HandleRotation();
        HandleJumpingMovement();
        HandleAirborneMovement();
    }
    private void HandleGroundedMovement()
    {
        if (player.canMove || player.canRotate)
        {
            GetVertandHoriInputs();
        }
        if (!player.canMove) return; //if i cant move, dont let me move

        //For move direction based on camera perspective and inputs
        moveDirection = PlayerCamera.instance.transform.forward * verticalMovement;
        moveDirection += PlayerCamera.instance.transform.right * horizontalMovement;
        moveDirection.Normalize();
        moveDirection.y = 0;

        ////For movement independant of camera, determine which direction to move here
        //moveDirection = player.transform.forward * verticalMovement;
        //moveDirection += player.transform.right * horizontalMovement;
        //moveDirection.Normalize();
        //moveDirection.y = 0;

        //now determine SPEED of movement
        if(isSprinting)
        {
            player.characterController.Move(moveDirection * sprintSpeed * Time.deltaTime);
        }
        else
        {
            if (InputManager.instance.moveAmount > 0.5f)
            {
                //running speed
                player.characterController.Move(moveDirection * runSpeed * Time.deltaTime);
            }
            else if (InputManager.instance.moveAmount <= 0.5f)
            {
                //walking
                player.characterController.Move(moveDirection * walkSpeed * Time.deltaTime);
            }
        }


    }
    private void GetVertandHoriInputs()
    {
        verticalMovement = InputManager.instance.vertical_Input;
        horizontalMovement = InputManager.instance.horizontal_Input;
        moveAmount = InputManager.instance.moveAmount;

        //clamp for animations
    }
    private void HandleRotation()
    {
        if (player.isDead) return;
        if (!player.canRotate) return; //if i cant rotate, dont let me rotate

        if (player.playerCombatManager.isLockedOn)//if locked on 
        {
            //allow sprinting/rolling without facing the target while locked on
            if (isSprinting || isDodging)
            {
                Vector3 targetDirection = Vector3.zero;
                targetDirection = PlayerCamera.instance.vCam.transform.forward * verticalMovement;
                targetDirection += PlayerCamera.instance.vCam.transform.right * horizontalMovement;
                targetDirection.Normalize();
                targetDirection.y = 0;

                if (targetDirection == Vector3.zero)
                {
                    targetDirection = transform.forward;
                }

                Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
                Quaternion finalRotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
                transform.rotation = finalRotation;
            }
            else //if locked on and not sprinting, we need to be strafing instead of normal movement 
            {
                if (player.playerCombatManager.currentTarget == null) return;

                Vector3 targetDirection;
                targetDirection = player.playerCombatManager.currentTarget.transform.position - player.transform.position;
                targetDirection.y = 0;
                targetDirection.Normalize();

                Debug.DrawLine(player.playerCombatManager.currentTarget.transform.position, player.transform.position, Color.red);

                Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
                Quaternion finalRotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
                player.transform.rotation = finalRotation;

            }
        }
        else
        {
            targetRotateDirection = Vector3.zero;
            targetRotateDirection = PlayerCamera.instance.vCam.transform.forward * verticalMovement;
            targetRotateDirection += PlayerCamera.instance.vCam.transform.right * horizontalMovement;
            targetRotateDirection.Normalize();
            targetRotateDirection.y = 0;

            if (targetRotateDirection == Vector3.zero)
            {
                targetRotateDirection = transform.forward;
            }

            Quaternion newRotation = Quaternion.LookRotation(targetRotateDirection);
            Quaternion targetRotation = Quaternion.Slerp(transform.rotation, newRotation, rotationSpeed * Time.deltaTime);
            transform.rotation = targetRotation;
        }

    }
    private void HandleJumpingMovement()
    {
        if (isJumping)
        {
            player.characterController.Move(jumpDirection * jumpForwardSpeed * Time.deltaTime);
        }
    }
    private void HandleAirborneMovement()
    {
        if (!player.isGrounded)
        {
            Vector3 airborneDirection;
            airborneDirection = PlayerCamera.instance.vCam.transform.forward * InputManager.instance.vertical_Input;
            airborneDirection += PlayerCamera.instance.vCam.transform.right * InputManager.instance.horizontal_Input;
            airborneDirection.y = 0;

            player.characterController.Move(airborneDirection * airborneManeuverSpeed * Time.deltaTime);
        }
    }

    public void HandleSprinting()
    {
        if (player.isPerformingAction)
        {
            //if doing something else, set sprinting to false
            isSprinting = false;
        }
        //if out of stamina stop sprinting
        if (player.playerStatsManager.currentStamina.GetFloat() <= 0)
        {
            isSprinting = false;
            return;
        }

        if (moveAmount >= 0.5f)
        {
            isSprinting = true;
        }
        else
        {
            isSprinting = false;
        }
        if (isSprinting)
        {
            player.playerStatsManager.currentStamina.SetFloat(
                player.playerStatsManager.currentStamina.GetFloat() 
                - sprintStaminaCost * Time.deltaTime);
        }
    }
    public void AttemptToDodge()
    {
        //check for other actions and stamina
        if (player.isPerformingAction) return;
        if (player.playerStatsManager.currentStamina.GetFloat() <= 0) return;

        if (moveAmount > 0) //if moving, dodge in direction of movement
        {
            dodgeDirection = PlayerCamera.instance.vCam.transform.forward * verticalMovement;
            dodgeDirection += PlayerCamera.instance.vCam.transform.right * horizontalMovement;
            dodgeDirection.y = 0;
            dodgeDirection.Normalize();

            Quaternion playerRotation = Quaternion.LookRotation(dodgeDirection);
            player.transform.rotation = playerRotation;

            player.playerAnimatorManager.PlayTargetActionAnimation("Fwd_Dodge_01", true, true, false, false);

            player.playerStatsManager.currentStamina.SetFloat(
                player.playerStatsManager.currentStamina.GetFloat() - dodgeStaminaCost);

            isDodging = true;
        }
        else //if stationary, dodge backwards (backstep)
        {
            if (player.isGrounded) //roll allowed in air, backstep is not 
            {
                player.playerAnimatorManager.PlayTargetActionAnimation("Fwd_Dodge_01", true, true, false, false);

                player.playerStatsManager.currentStamina.SetFloat(
                    player.playerStatsManager.currentStamina.GetFloat() - dodgeStaminaCost);

                isDodging = true;
            }
        }
    }
    public void AttemptToJump()
    {
        //no jumping is doing another action (can be changed to allow attacks)
        if (player.isPerformingAction) return;

        //no jump if out of stamina
        if (player.playerStatsManager.currentStamina.GetFloat() <= 0) return;

        //no jump if we are already jumping
        if (isJumping) return;

        //no jumping if we are not on the ground
        if (!player.isGrounded) return;

        //play animation depending on which weapon/how many weapons we are using etc
        player.playerAnimatorManager.PlayTargetActionAnimation("SS_Main_Jump_Start_01", false, true);
        isJumping = true;

        player.playerStatsManager.currentStamina.SetFloat(
            player.playerStatsManager.currentStamina.GetFloat() - jumpStaminaCost);

        jumpDirection = PlayerCamera.instance.vCam.transform.forward * InputManager.instance.vertical_Input;
        jumpDirection += PlayerCamera.instance.vCam.transform.right * InputManager.instance.horizontal_Input;
        jumpDirection.y = 0;

        if (jumpDirection != Vector3.zero)
        {
            //our movement speed will affect how much we can move while in the air 
            if (isSprinting)
            {
                jumpDirection *= 2;
            }
            else if (moveAmount > 0.5f)
            {
                jumpDirection *= 1f;
            }
            else if (moveAmount <= 0.5f)
            {
                jumpDirection *= 0.25f;
            }
        }
    }
    public void ApplyJumpForce()
    {
        //apply an upward velocity, depends on in game forces
        Debug.Log("Jump force Applied!");
        yVelocity.y = Mathf.Sqrt(jumpHeight * -2 * gravityForce);
    }
}
