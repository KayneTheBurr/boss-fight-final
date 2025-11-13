using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class CharacterManager : MonoBehaviour
{
    
    [HideInInspector] public CharacterController characterController;
    [HideInInspector] public CharacterCombatManager characterCombatManager;
    [HideInInspector] public CharacterMovementManager characterMovementManager;
    [HideInInspector] public Animator animator;
    [HideInInspector] public CharacterAnimationManager characterAnimationManager;
    [HideInInspector] public CharacterStatManager characterStatManager;
    [HideInInspector] public CharacterEffectsManager characterEffectsManager;
    [HideInInspector] public CharacterSFXManager characterSFXManager;
    [HideInInspector] public CharacterUIManager characterUIManager;

    [Header("Character Name")]
    public ObservableVariable characterName = new ObservableVariable("");

    [Header("Character Group")]
    public CharacterGroup characterGroup;

    [Header("Character Flags")]
    public bool isDead = false;
    public bool isPerformingAction;
    public bool isGrounded = true;
    public bool canRotate = true;
    public bool canMove = true;
    public bool applyRootMotion = false;

    [Header("Animator Values")]
    public float horizontalMovement;
    public float verticalMovement;
    public float moveAmount;

    
    protected virtual void Awake()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        
        characterCombatManager = GetComponent<CharacterCombatManager>();
        characterMovementManager = GetComponent<CharacterMovementManager>();
        characterAnimationManager = GetComponent<CharacterAnimationManager>();
        characterStatManager = GetComponent<CharacterStatManager>();
        characterSFXManager = GetComponent<CharacterSFXManager>();
        characterUIManager = GetComponent<CharacterUIManager>();
        characterEffectsManager = GetComponent<CharacterEffectsManager>();
    }
    protected virtual void Start()
    {
        IgnoreMyOwnColliders();
    }
    protected virtual void OnEnable()
    {
        
    }
    protected virtual void OnDisable()
    {

    }
    protected virtual void Update()
    {
        animator.SetBool("isGrounded", isGrounded);
    }
    protected virtual void FixedUpdate()
    {

    }
    protected virtual void IgnoreMyOwnColliders()
    {
        //get all the colliders on us(character controller) and children (damagable bone colliders)
        Collider characterControllerCollider = GetComponent<Collider>();
        Collider[] damagableCharacterColliders = GetComponentsInChildren<Collider>();

        //make a list of colliders to ignore and add all owners own colliders to the list to ignore
        List<Collider> collidersToIgnore = new List<Collider>();
        foreach (var collider in damagableCharacterColliders)
        {
            collidersToIgnore.Add(collider);
        }
        collidersToIgnore.Add(characterControllerCollider);

        //go through every collider on the list and make them ignore one another
        foreach (var collider in collidersToIgnore)
        {
            foreach (var otherCollider in collidersToIgnore)
            {
                Physics.IgnoreCollision(collider, otherCollider, true);
            }
        }

    }

    public void OnIsChargingAttackChanged(bool old, bool isChargingAttack)
    {
        animator.SetBool("IsChargingAttack", isChargingAttack);
    }
    public void OnIsMovingChanged(bool oldStatus, bool newStatus)
    {
        //Debug.Log("IsMoving changed!");
        animator.SetBool("IsMoving", characterMovementManager.isMoving.GetBool());
    }
    public virtual IEnumerator HandleDeathEvents(bool manuallySelectDeathAnim = false)
    {

        characterStatManager.currentHealth.SetFloat(0);
        isDead = true;

        //reset all flags that need to be reset

        //if we are not grounded, play arial death animation
        if (!manuallySelectDeathAnim)
        {
            characterAnimationManager.PlayTargetActionAnimation("SS_Main_Dead_01", true);
        }
        //play death vfx/sfx

        yield return new WaitForSeconds(5);

        //award players some currency for slaying enemy 

        //disable the character

    }

}
