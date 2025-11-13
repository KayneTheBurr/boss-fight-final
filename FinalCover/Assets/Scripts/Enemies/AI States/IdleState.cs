using UnityEngine;

[CreateAssetMenu(menuName = "AI / States / Idle")]
public class IdleState : AIStates
{
    public override AIStates Tick(EnemyCharacterManager enemy)
    {
        if (enemy.characterCombatManager.currentTarget != null)
        {
            //return the pursue target state instead
            enemy.inCombat = true;
            enemy.HandlePlayerHUDBossUI(enemy.inCombat);
            enemy.animator.SetBool("InCombatStance", true);
            return SwitchState(enemy, enemy.pursueTarget);
        }
        else
        {
            //return the same (idle) state, keep searching for a target
            enemy.inCombat = false;
            enemy.HandlePlayerHUDBossUI(enemy.inCombat);
            enemy.animator.SetBool("InCombatStance", false);
            enemy.enemyCombatManager.FindATargetViaLineOfSight(enemy);
            return this;
        }
    }
}
