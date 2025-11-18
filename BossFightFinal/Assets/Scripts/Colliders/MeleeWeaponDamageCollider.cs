using UnityEngine;

public class MeleeWeaponDamageCollider : DamageCollider
{
    [Header("Attacking Character")]
    public CharacterManager characterCausingDamage; //lets us check for the attackers damage modifiers etc 

    [Header("Weapon Attack Modifiers")]
    public float light_Attack_01_DamageModifier;
    public float light_Attack_02_DamageModifier;
    public float heavy_Attack_01_DamageModifier;
    public float heavy_Attack_02_DamageModifier;
    public float charge_Attack_01_DamageModifier;
    public float charge_Attack_02_DamageModifier;
    public float light_Run_Attack_01_DamageModifier;
    public float light_Roll_Attack_01_DamageModifier;
    public float light_BackStep_Attack_01_DamageModifier;

    protected override void Awake()
    {
        base.Awake();

        if (damageCollider == null)
        {
            damageCollider = GetComponent<Collider>();
        }

        //melee damage colliders only on during attack animations 
        damageCollider.enabled = false;
    }

    protected override void OnTriggerEnter(Collider col)
    {
        
        CharacterManager damageTarget = col.GetComponentInParent<CharacterManager>();

        if (damageTarget != null)
        {
            if (damageTarget == characterCausingDamage) return; //dont let us hit ourselves with our attacks 

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
        //dont want to deal damage again to a target if we already damaged them with this instance of damage
        //add them to a list and check the list to see if they are on the list of damageable characters already or not 
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
        damageEffect.angleHitFrom = Vector3.SignedAngle(characterCausingDamage.transform.forward, damageTarget.transform.forward, Vector3.up);


        switch (characterCausingDamage.characterCombatManager.currentAttackType)
        {
            case AttackType.LightAttack01:
                ApplyAttackDamageModifiers(light_Attack_01_DamageModifier, damageEffect);
                break;
            case AttackType.LightAttack02:
                ApplyAttackDamageModifiers(light_Attack_02_DamageModifier, damageEffect);
                break;
            case AttackType.HeavyAttack01:
                ApplyAttackDamageModifiers(heavy_Attack_01_DamageModifier, damageEffect);
                break;
            case AttackType.HeavyAttack02:
                ApplyAttackDamageModifiers(heavy_Attack_02_DamageModifier, damageEffect);
                break;
            case AttackType.ChargeAttack01:
                ApplyAttackDamageModifiers(charge_Attack_01_DamageModifier, damageEffect);
                break;
            case AttackType.ChargeAttack02:
                ApplyAttackDamageModifiers(charge_Attack_02_DamageModifier, damageEffect);
                break;
            case AttackType.LightRunningAttack01:
                ApplyAttackDamageModifiers(light_Run_Attack_01_DamageModifier, damageEffect);
                break;
            case AttackType.LightRollingAttack01:
                ApplyAttackDamageModifiers(light_Roll_Attack_01_DamageModifier, damageEffect);
                break;
            case AttackType.LightBackStepAttack01:
                ApplyAttackDamageModifiers(light_BackStep_Attack_01_DamageModifier, damageEffect);
                break;
            default:
                break;
        }
        //Debug.Log("Dealt: " + damageEffect.physicalDamage + " damage!");
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

