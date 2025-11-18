using UnityEngine;

[CreateAssetMenu(menuName = "AI / States / Attack ")]
public class AttackState : AIStates
{
    [Header("Current Attack")]
    [SerializeField] public EnemyAttackAction currentAttack;
    [HideInInspector] public bool willPerformCombo = false;

    [Header("State Flags")]
    [SerializeField] protected bool hasPerformedAttack = false;
    protected bool hasPerformedCombo = false;

    [Header("Pivot After Attack")]
    [SerializeField] protected bool pivotAfterAttack = false;

    public override AIStates Tick(EnemyCharacterManager enemy)
    {
        if (enemy.enemyCombatManager.currentTarget == null) // go to idle if target is null 
            return SwitchState(enemy, enemy.idle);

        if (enemy.enemyCombatManager.currentTarget.isDead) // go to idle if target is dead
            return SwitchState(enemy, enemy.idle);

        enemy.enemyCombatManager.RotateTowardsTargetWhileAttacking(enemy, currentAttack);

        enemy.characterAnimationManager.UpdateAnimatorMovementParameters(0, 0, false);

        //perform a combo 
        if (willPerformCombo && !hasPerformedCombo)
        {
            //
            if (currentAttack.comboAction != null)
            {
                //if can combo 
                //hasPerformedCombo = true;
                //currentAttack.comboAction.AttemptToPerformAction(aiCharacter);
            }
        }

        if (enemy.isPerformingAction) return this;

        //
        if (!hasPerformedAttack)
        {
            if (enemy.enemyCombatManager.actionRecoveryTimer > 0) return this;

            PerformAttack(enemy);

            //return to top so that we can combo if we are able to 
            return this;
        }

        if (pivotAfterAttack)
            enemy.enemyCombatManager.PivotTowardsTarget(enemy);

        return SwitchState(enemy, enemy.combatStance);

    }
    protected void PerformAttack(EnemyCharacterManager enemy)
    {
        hasPerformedAttack = true;
        currentAttack.AttemptToPerformAction(enemy);

        //set the recovery timer to the time associated with its current attack value 
        enemy.enemyCombatManager.actionRecoveryTimer = currentAttack.actionRecoveryTime;
        enemy.enemyCombatManager.StartCooldown(currentAttack);
    }
    protected override void ResetStateFlags(EnemyCharacterManager aiCharacter)
    {
        base.ResetStateFlags(aiCharacter);
        hasPerformedAttack = false;
        hasPerformedCombo = false;

    }
}
