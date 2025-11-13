using UnityEngine;

public class EnemyAnimationManager : CharacterAnimationManager
{
    EnemyCharacterManager aiCharacter;

    protected override void Awake()
    {
        base.Awake();
        aiCharacter = GetComponent<EnemyCharacterManager>();
    }
    private void OnAnimatorMove()
    {
        if (!aiCharacter.isGrounded) return;

        Vector3 velocity = aiCharacter.animator.deltaPosition;

        aiCharacter.characterController.Move(velocity);
        aiCharacter.transform.rotation *= aiCharacter.animator.deltaRotation;

    }
}
