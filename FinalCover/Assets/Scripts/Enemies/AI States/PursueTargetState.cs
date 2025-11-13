using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(menuName = "AI / States / Pursue")]
public class PursueTargetState : AIStates
{
    public override AIStates Tick(EnemyCharacterManager enemy)
    {
        //check if we are performing an action(if so do nothing until the action is done)
        if (enemy.isPerformingAction) return this;

        //check if our target is null, if we do not have a target, return to idle state
        if (enemy.enemyCombatManager.currentTarget == null)
            return SwitchState(enemy, enemy.idle);

        //make sure navmeshagent is active, if not enable it 
        if (enemy.navMeshAgent.enabled == false)
        {
            enemy.navMeshAgent.enabled = true;
        }

        //if our target is outside of our FOV, pivot to face them 
        if (enemy.enemyCombatManager.viewableAngle < enemy.enemyCombatManager.minFOV ||
            enemy.enemyCombatManager.viewableAngle > enemy.enemyCombatManager.maxFOV)
        {
            enemy.enemyCombatManager.PivotTowardsTarget(enemy);
        }

        enemy.enemyMovementManager.RotateTowardsAgent(enemy);

        //option 1 (better for ranged enemies or melee/ranged hybrid enemies)
        
        if(enemy.enemyCombatManager.currentTarget && //has a target
            enemy.enemyCombatManager.HasRangedAttack() && //has a ranged attack
            enemy.enemyCombatManager.HasRangedAttackAvailable( //has a ranged attack available 
                enemy.transform.position, enemy.enemyCombatManager.currentTarget.transform.position))
        {
            //we have a ranged attack available, go to cvombat stance to use it 
            
            return SwitchState(enemy, enemy.combatStance);
        }

        //option 2 (melee enemies who will try to close the gap more)
        if (enemy.enemyCombatManager.distanceFromTarget <= enemy.navMeshAgent.stoppingDistance)
            return SwitchState(enemy, enemy.combatStance);

        //if target is not reachable and they are far away, return back to "home" area 

        //pursue the target
        //Debug.Log("Too far away, gotta get closer");
        NavMeshPath path = new NavMeshPath();
        enemy.navMeshAgent.CalculatePath(enemy.enemyCombatManager.currentTarget.transform.position, path);
        enemy.navMeshAgent.SetPath(path);
        return this;
    }
}
