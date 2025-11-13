using UnityEngine;

public class WeaponItem : Item
{
    [Header("Animations")]
    public AnimatorOverrideController weaponOverrideAnimator;

    [Header("Weapon Model")]
    public GameObject weaponModel;

    [Header("Weapon Class")]
    public WeaponClass weaponClass;

    [Header("Weapon Stat Requirements")]
    public int strREQ = 0;
    public int agiREQ = 0;
    public int arcREQ = 0;
    public int essREQ = 0;
    public int cunREQ = 0;
    public int omnREQ = 0;

    [Header("Weapon Base Damage")]
    public int physicalDamage = 0;
    public int fireDamage = 0;
    public int lightningDamage = 0;
    public int iceDamage = 0;
    public int poisonDamage = 0;
    public int decayDamage = 0;
    public int shadowDamage = 0;

    [Header("Light Attack Modifiers")]
    //stat scaling
    //weapon modifiers
    public float light_Attack_01_DamageModifier = 0.8f;
    public float light_Attack_02_DamageModifier = 0.9f;
    public float light_Attack_03_DamageModifier = 0.8f;
    public float light_Attack_04_DamageModifier = 0.9f;
    public float light_Attack_05_DamageModifier = 0.9f;
    [Header("Heavy Attack Modifiers")]
    public float heavy_Attack_01_DamageModifier = 1.3f;
    public float heavy_Attack_02_DamageModifier = 1.4f;
    public float heavy_Attack_03_DamageModifier = 1.3f;
    public float heavy_Attack_04_DamageModifier = 1.4f;
    public float heavy_Attack_05_DamageModifier = 1.4f;

    [Header("Charge Attack Modifiers")]
    public float charge_Attack_01_DamageModifier = 2.0f;
    public float charge_Attack_02_DamageModifier = 2.2f;
    public float charge_Attack_03_DamageModifier = 2.0f;
    public float charge_Attack_04_DamageModifier = 2.2f;
    public float charge_Attack_05_DamageModifier = 2.2f;

    [Header("Movement Attack Modifiers")]
    public float light_Run_Attack_01_DamageModifier = 0.75f;
    public float light_Roll_Attack_01_DamageModifier = 1.0f;
    public float light_BackStep_Attack_01_DamageModifier = 1.4f;

    //critical modifiers

    [Header("Weapon Base Poise Damage")]
    public float poiseDamage = 10;
    //weapon hyperarmor?

    //weapon blocking absorbtion and stagger

    [Header("Stamina Cost Modifiers")]
    public int baseStaminaCost = 20;
    public float lightAttackStaminaCostModifier = 1.1f;
    public float heavyAttackStaminaCostModifier = 1.4f;
    public float chargeAttackStaminaCostModifier = 1.8f;
    public float lightRunAttackStaminaCostModifier = 1.3f;
    public float lightRollAttackStaminaCostModifier = 1.0f;
    public float lightBackStepAttackStaminaCostModifier = 0.6f;

    //item based actions
    [Header("Item Based Actions")]
    public WeaponItemAction oh_r1_Action; //one handed r1 weapon action 
    public WeaponItemAction oh_r2_Action; // one handed r2 heavy attack action 

    //special abilities

    //blocking 


    //SFX here when swinging weapon item 
    [Header("Whooshes")]
    public AudioClip[] whooshes;
}
