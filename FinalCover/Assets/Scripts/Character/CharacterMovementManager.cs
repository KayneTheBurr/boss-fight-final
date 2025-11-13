using UnityEngine;

public class CharacterMovementManager : MonoBehaviour
{
    public CharacterManager character;

    [Header("Ground Checks And Jumping")]
    [SerializeField] float groundCheckSphereRadius = 1f;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] protected float gravityForce = -9.81f;

    [SerializeField] protected Vector3 yVelocity; //measure the force characters are pulled downward
    [SerializeField] protected float groundedYVelocity = -20; //sticks us to the ground while grounded
    [SerializeField] protected float fallStartYVelocity = -5; //down force at the START of our fall, rises over time 
    [SerializeField] protected float groundCheckYOffset = 0f; 
    protected bool fallingVelocitySet = false;
    protected float inAirTimer = 0;
    

    [Header("Movement Flags")]
    public bool isSprinting = false;
    public ObservableVariable isMoving = new ObservableVariable(false);
    public bool isJumping = false;
    public bool isDodging = false;
    public bool isFlying = false;

    protected virtual void Awake()
    {
        character = GetComponent<CharacterManager>();
    }
    protected virtual void OnEnable()
    {
        //character.animator.SetBool("IsMoving", isMoving.GetBool());
        isMoving.OnBoolChanged += character.OnIsMovingChanged;
    }
    protected virtual void OnDisable()
    {
        isMoving.OnBoolChanged -= character.OnIsMovingChanged;
    }
    protected virtual void Update()
    {
        if (HandleGroundChecks())
        {
            //if we are not attempting to jump or are falling
            if (yVelocity.y < 0)
            {
                inAirTimer = 0;
                fallingVelocitySet = false;
                yVelocity.y = groundedYVelocity;
            }
        }
        else //if we are not grounded do this 
        {
            if (!isJumping && !fallingVelocitySet) //if not jumping AND falling speed not set
            {
                fallingVelocitySet = true;
                yVelocity.y = fallStartYVelocity;
            }
            inAirTimer += Time.deltaTime;
            character.animator.SetFloat("InAirTimer", inAirTimer);
            yVelocity.y += gravityForce * Time.deltaTime;
        }
        //always apply force downward on the player regardless of grounded or not unless flying
        if (!isFlying)
        {
            character.characterController.Move(yVelocity * Time.deltaTime);
        }
        else
        { 
            //Flying, do things different here
        }
    }
    
    public void EnableCanRotate()
    {
        //Debug.Log("Enabled Can Rotate!");
        character.canRotate = true;
    }

    public void DisableCanRotate()
    {
        character.canRotate = false;
    }

    protected virtual bool HandleGroundChecks()
    {
        character.isGrounded = Physics.CheckSphere(
            character.transform.position + new Vector3(0, groundCheckYOffset, 0), groundCheckSphereRadius, groundLayer);
        return character.isGrounded;
    }

    protected void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(character.transform.position + new Vector3(0, groundCheckYOffset, 0), groundCheckSphereRadius);
    }
}
