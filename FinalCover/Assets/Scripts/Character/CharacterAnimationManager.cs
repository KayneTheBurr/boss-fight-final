using UnityEngine;

public class CharacterAnimationManager : MonoBehaviour
{
    CharacterManager character;
    int vertical;
    int horizontal;

    [Header("Damage Animations")]
    public string lastDamageAnimationPlayed;

    protected virtual void Awake()
    {
        character = GetComponent<CharacterManager>();
        vertical = Animator.StringToHash("Vertical");
        horizontal = Animator.StringToHash("Horizontal");
    }
    public void UpdateAnimatorMovementParameters(float horizontalValue, float verticalValue, bool isSprinting)
    {
        float snappedHorizontalAmt;
        float snappedVerticalAmt;

        //snap horizontal values 
        if (horizontalValue > 0 && horizontalValue <= 0.5f)
        {
            snappedHorizontalAmt = 0.5f;
        }
        else if (horizontalValue > 0.5f && horizontalValue <= 1f)
        {
            snappedHorizontalAmt = 1f;
        }
        else if (horizontalValue < 0 && horizontalValue >= -0.5f)
        {
            snappedHorizontalAmt = -0.5f;
        }
        else if (horizontalValue < 0 && horizontalValue >= -1f)
        {
            snappedHorizontalAmt = -1f;
        }
        else snappedHorizontalAmt = 0;

        //snap vertical values
        if (verticalValue > 0 && verticalValue <= 0.5f)
        {
            snappedVerticalAmt = 0.5f;
        }
        else if (verticalValue > 0.5f && verticalValue <= 1f)
        {
            snappedVerticalAmt = 1f;
        }
        else if (verticalValue < 0 && verticalValue >= -0.5f)
        {
            snappedVerticalAmt = -0.5f;
        }
        else if (verticalValue < 0 && verticalValue >= -1f)
        {
            snappedVerticalAmt = -1f;
        }
        else snappedVerticalAmt = 0;

        if (isSprinting)
        {
            snappedVerticalAmt = 2;
        }
        character.animator.SetFloat(horizontal, snappedHorizontalAmt, 0.1f, Time.deltaTime);
        character.animator.SetFloat(vertical, snappedVerticalAmt, 0.1f, Time.deltaTime);
    }

    public virtual void PlayTargetActionAnimation(string targetAnimation,
                            bool isPerformingAction, bool applyRootMotion = true,
                            bool canRotate = false, bool canMove = false)
    {
        
        //stop character from performing other actions
        character.isPerformingAction = isPerformingAction;
        character.canMove = canMove;
        character.canRotate = canRotate;

        character.applyRootMotion = applyRootMotion;
        character.animator.CrossFade(targetAnimation, 0.2f);

    }
    public virtual void PlayTargetAttackActionAnimation(WeaponItem weapon, AttackType attackType,
                            string targetAnimation,
                            bool isPerformingAction, bool applyRootMotion = true,
                            bool canRotate = false, bool canMove = false)
    {
        //Debug.Log(attackType); //called successfuly

        //keep track of the last attack performed
        //keep track of current attack type
        //update the animation set to current weapon animations
        //decide if our attack can be parried
        //tell the network we are attacking flag (counter damage etc)

        character.characterCombatManager.currentAttackType = attackType;
        character.characterCombatManager.lastAttackAnimation = targetAnimation;
        
        character.isPerformingAction = isPerformingAction;
        character.canMove = canMove;
        character.canRotate = canRotate;

        if (weapon?.weaponOverrideAnimator != null)
            UpdateAnimatorController(weapon.weaponOverrideAnimator);

        character.applyRootMotion = applyRootMotion;
        character.animator.CrossFade(targetAnimation, 0.2f);

    }

    public void UpdateAnimatorController(AnimatorOverrideController weaponController)
    {
        character.animator.runtimeAnimatorController = weaponController;
    }


}
