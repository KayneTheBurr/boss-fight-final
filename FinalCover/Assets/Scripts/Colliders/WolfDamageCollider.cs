using Unity.VisualScripting;
using UnityEngine;

public class WolfDamageCollider : DamageCollider
{
    public EnemyCharacterManager wolfCharacter;

    protected override void Awake()
    {
        base.Awake();
        damageCollider = GetComponent<Collider>();
        wolfCharacter = GetComponentInParent<EnemyCharacterManager>();
    }
    protected override void OnTriggerEnter(Collider col)
    {
        CharacterManager damageTarget = col.GetComponentInParent<CharacterManager>();

        if (damageTarget != null)
        {
            if (damageTarget == wolfCharacter) return; //dont let us hit ourselves with our attacks 

            contactPoint = damageTarget.GetComponent<Collider>().ClosestPointOnBounds(transform.position);

            //check if we can damage this target or not based on characters "freindly fire" 
            //check if target is blocking

            //check if target is invulnerable
            if (damageTarget.characterCombatManager.isInvulnerable) return;


            DamageTarget(damageTarget);
        }
    }
    protected override void DamageTarget(CharacterManager damageTarget)
    {
        if (charactersDamaged.Contains(damageTarget)) return;
        charactersDamaged.Add(damageTarget);

        TakeDamageEffect damageEffect = Instantiate(WorldCharacterEffectsManager.instance.takeDamageEffect);

        damageEffect.physicalDamage = physicalDamage;
        damageEffect.fireDamage = fireDamage;
        damageEffect.lightningDamage = lightningDamage;
        damageEffect.iceDamage = iceDamage;
        damageEffect.poisonDamage = poisonDamage;
        damageEffect.shadowDamage = shadowDamage;
        damageEffect.decayDamage = decayDamage;

        damageEffect.contactPoint = contactPoint;
        damageEffect.angleHitFrom = Vector3.SignedAngle(wolfCharacter.transform.forward, damageTarget.transform.forward, Vector3.up);

        var cm = wolfCharacter.GetComponent<WolfCombatManager>();

        switch (wolfCharacter.characterCombatManager.currentAttackType)
        {
            case AttackType.LightAttack01:
                ApplyAttackDamageModifiers(cm.swipeAttack_01_DamageModifier, damageEffect);
                break;
            case AttackType.LightAttack02:
                ApplyAttackDamageModifiers(cm.swipeAttack_01_DamageModifier, damageEffect);
                break;
            case AttackType.LightAttack03:
                ApplyAttackDamageModifiers(cm.sideBiteAttack_01_DamageModifier, damageEffect);
                break;
            case AttackType.LightAttack04:
                ApplyAttackDamageModifiers(cm.sideBiteAttack_01_DamageModifier, damageEffect);
                break;
            case AttackType.HeavyAttack01:
                ApplyAttackDamageModifiers(cm.lungeBiteAttack_01_DamageModifier, damageEffect);
                break;
            default:
                break;
        }

        damageTarget.characterEffectsManager.ProcessInstantEffects(damageEffect);
    }
    private void ApplyAttackDamageModifiers(float modifier, TakeDamageEffect damage)
    {
        damage.physicalDamage *= modifier;
        damage.fireDamage *= modifier;
        damage.lightningDamage *= modifier;
        damage.iceDamage *= modifier;
        damage.poisonDamage *= modifier;
        damage.shadowDamage *= modifier;
        damage.decayDamage *= modifier;

        damage.poiseDamage *= modifier;

        //if the attack is fully charged, multiply by full charge modifier AFTER normal modifiers 

    }
}
