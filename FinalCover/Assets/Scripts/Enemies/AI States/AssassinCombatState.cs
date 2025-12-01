using UnityEngine;

[CreateAssetMenu(menuName = "AI / States / Assassin Combat Stance")]
public class AssassinCombatState : CombatStanceState
{
    
    public override AIStates Tick(EnemyCharacterManager enemy)
    {
        var cm = enemy.enemyCombatManager as AssassinEnemyCombatManager;
        if(cm == null)
        {
            Debug.LogError($"Assassin combat state for {enemy.characterName} requires an Assassin Combat Manager!");
            return base.Tick(enemy);
        }

        if (cm.currentTarget == null || cm.currentTarget.isDead)
            return SwitchState(enemy, enemy.idle);

        if (enemy.isPerformingAction) return this;

        if (!enemy.navMeshAgent.enabled)
            enemy.navMeshAgent.enabled = true;

        bool hasRangedReady = cm.HasRangedAttack() && cm.HasRangedAttackAvailable(
            enemy.transform.position, cm.currentTarget.transform.position);

        if (!hasRangedReady && cm.distanceFromTarget > maxEngagementDistance)
        {
            return SwitchState(enemy, enemy.pursueTarget);
        }

        if (!enemy.enemyMovementManager.isMoving.GetBool())
        {
            if (Mathf.Abs(cm.viewableAngle) > turnThreshold)
            {
                cm.PivotTowardsTarget(enemy);
                return this; //rotate this tick then try again
            }
        }

        cm.HandleAssassinMovement(enemy);

        //maybe see what player is doing and try to dodge here?
        if(cm.currentTarget.isPerformingAction && cm.TryDodge(enemy))
        {
            cm.PerformDodge(enemy);
        }

        Debug.DrawLine(enemy.transform.position + Vector3.up,
               enemy.transform.position + enemy.transform.right * 2f, Color.magenta);

        //// choose attack
        //var newAttack = GetNewAttack(enemy);
        //if (newAttack != null)
        //{
        //    chosenAttack = newAttack;
        //    previousAttack = chosenAttack;
        //    hasAttacked = true;

        //    enemy.attack.currentAttack = chosenAttack;
        //    return SwitchState(enemy, enemy.attack);
        //}

        // return to pursue if the player gets to far away
        if (cm.distanceFromTarget > maxEngagementDistance)
            return SwitchState(enemy, enemy.pursueTarget);

        //else stay in combat until attack made
        return this;
    }

    
}
