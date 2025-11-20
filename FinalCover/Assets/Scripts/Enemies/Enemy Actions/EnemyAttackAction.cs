using UnityEngine;

[CreateAssetMenu(menuName = "AI / Actions / Attack ")]
public class EnemyAttackAction : ScriptableObject
{
    [Header("Attack")]
    [SerializeField] private string attackAnimation;
    [SerializeField] public EnemyAttackType enemyAttackType;
    [SerializeField] public AttackType attackType;

    [Header("Combo Actions")]
    public EnemyAttackAction comboAction; //the combo action of this attack action

    [Header("Attack Rotation Speed")]
    public float attackTrackingSpeed = 20;

    [Header("Action Values")]
    public int attackWeight;
    public float cooldownTime;
    public bool requiresLOS = true;
    public float actionRecoveryTime = 1.5f;
    public float minAttackAngle = -35;
    public float maxAttackAngle = 35;
    public float minAttackDistance = 0;
    public float maxAttackDistance = 3;

    public void AttemptToPerformAction(EnemyCharacterManager enemy)
    {
        enemy.characterAnimationManager.PlayTargetActionAnimation(attackAnimation, true);
        
    }
}
