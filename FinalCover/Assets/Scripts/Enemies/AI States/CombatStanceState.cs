using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.UI.Image;

[CreateAssetMenu(menuName = "AI / States / Combat Stance")]
public class CombatStanceState : AIStates
{
    //1. Select an attack for the attack state based on: Distance, Angle, Chance/weight, probability etc
    //2. process combat logic while waiting to attack(dodge/block/strafe etc)
    //3. if target moves out of range, return to pursue state to chase
    //4. if target is too far away/no longer there, switch to idle state

    [Header("Attacks")]
    public List<EnemyAttackAction> enemyCharacterAttacks; //list of all possible attacks the character CAN do
    private List<EnemyAttackAction> potentialAttacks = new(); //all attacks possible in the situation based on angle/distance
    private List<int> weights = new();
    [SerializeField] private EnemyAttackAction chosenAttack;
    [SerializeField] private EnemyAttackAction previousAttack;
    protected bool hasAttacked = false;

    [Header("Combo")]
    [SerializeField] protected bool canPerformCombo = false;
    [SerializeField] protected int chanceToPerformCombo = 25; //0-100 for if enemy will perform combo attack 
    protected bool hasRolledForComboChance = false;

    [Header("Engagement Distance")]
    public float maxEngagementDistance = 5; //The distance we have to be away from target before returning to pursue state

    [Header("Tunable Constants")]
    [SerializeField] float meleePreferDist = 2.5f;
    [SerializeField] float rangedPreferDist = 5f;
    [SerializeField] float pivotAngleThreshold = 35f;
    [SerializeField] int weightCloseMelee = 2;
    [SerializeField] int weightFarRanged = 2;
    [SerializeField] int repeatPenalty = 1;

    public override AIStates Tick(EnemyCharacterManager enemy)
    {
        var cm = enemy.enemyCombatManager;

        //if our target is no longer there, switch back to idle
        if (cm.currentTarget == null ||
            cm.currentTarget.isDead)
        {
            return SwitchState(enemy, enemy.idle);
        }

        if (enemy.isPerformingAction) return this;

        if (!enemy.navMeshAgent.enabled)
            enemy.navMeshAgent.enabled = true; //turn on nev mesh if it was off 

        bool hasRangedReady = cm.HasRangedAttack() && cm.HasRangedAttackAvailable(
            enemy.transform.position, cm.currentTarget.transform.position);

        if (!hasRangedReady && cm.distanceFromTarget > maxEngagementDistance)
        {
            return SwitchState(enemy, enemy.pursueTarget);
        }

        //get the ai character to turn and face the target when its outisde its FOV
        if (!enemy.enemyMovementManager.isMoving.GetBool())
        {
            if (Mathf.Abs(cm.viewableAngle) > 35f)
            {
                cm.PivotTowardsTarget(enemy);
                return this; //rotate this tick then try again
            }
        }

        //rotate to face our target
        cm.RotateTowardsAgent(enemy);

        var pick = GetNewAttack(enemy);

        if (pick != null)
        {
            chosenAttack = pick;
            previousAttack = chosenAttack;
            hasAttacked = true;

            enemy.attack.currentAttack = chosenAttack;
            return SwitchState(enemy, enemy.attack);
        }


        //if we get outside of the combat state range, return to pursue state
        if (cm.distanceFromTarget > maxEngagementDistance)
            return SwitchState(enemy, enemy.pursueTarget);

        enemy.navMeshAgent.SetDestination(cm.currentTarget.transform.position);

        //NavMeshPath path = new NavMeshPath();
        //enemy.navMeshAgent.CalculatePath(cm.currentTarget.transform.position, path);
        //enemy.navMeshAgent.SetPath(path);
        return this;
    }
    protected virtual EnemyAttackAction GetNewAttack(EnemyCharacterManager enemy)
    {
        var cm = enemy.enemyCombatManager;

        if (cm.currentTarget == null ||
            cm.availableAttacks == null ||
            cm.availableAttacks.Count == 0) return null;

        potentialAttacks.Clear();
        weights.Clear();

        float dist = cm.distanceFromTarget;
        float angle = cm.viewableAngle;

        foreach (var attack in enemyCharacterAttacks)
        {
            if (attack == null) continue;
            if(!cm.AttackOffCooldown(attack)) continue;

            //check range from enemy to target 
            if (dist < attack.minAttackDistance || dist > attack.maxAttackDistance) continue;

            //check the attack angle, if outside FOV for the attack check the next attack
            if(angle < attack.minAttackAngle || angle > attack.maxAttackAngle) continue;

            //check if the attack needs LOS and has it or not 
            if (attack.requiresLOS && Physics.Linecast(
                enemy.transform.position, cm.currentTarget.transform.position,
                WorldUtilityManager.instance.GetEnviroLayers()))
            {
                continue;
            }
             
            potentialAttacks.Add(attack);
        }
        //try to turn?
        if (potentialAttacks.Count <= 0)
        {
            if(Mathf.Abs(angle) >= 35)
            {
                cm.PivotTowardsTarget(enemy);
                return null;
            }
            return null;
        }

        // weighted attack check, bias based on range 
        int totalWeight = 0;

        foreach (var attack in potentialAttacks)
        {
            //totalWeight += attack.attackWeight;
            int w = Mathf.Max(attack.attackWeight, 1);

            if (dist <= meleePreferDist && attack.enemyAttackType == EnemyAttackType.Melee)
                w += weightCloseMelee;

            if (dist >= rangedPreferDist && attack.enemyAttackType == EnemyAttackType.Ranged)
                w += weightFarRanged;

            if (previousAttack != null && attack == previousAttack && potentialAttacks.Count > 1 && w > 1)
                w -= repeatPenalty;

            weights.Add(w);
            totalWeight += w;

        }

        //Pick based on weights
        int roll = Random.Range(0, Mathf.Max(1, totalWeight));
        int acc = 0;
        for (int i = 0; i < potentialAttacks.Count; i++)
        {
            acc += weights[i];
            if (roll < acc) return potentialAttacks[i];
        }

        return potentialAttacks[0];
    }

    protected override void ResetStateFlags(EnemyCharacterManager aiCharacter)
    {
        base.ResetStateFlags(aiCharacter);

        hasRolledForComboChance = false;
        hasAttacked = false;
    }
}
