using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public MeleeWeaponDamageCollider meleeDamageCollider;

    private void Awake()
    {
        //Debug.Log("no col");
        meleeDamageCollider = GetComponentInChildren<MeleeWeaponDamageCollider>();
        //Debug.Log("col!");
    }
    public void SetWeaponDamage(CharacterManager characterWithWeapon, WeaponItem weapon)
    {
        meleeDamageCollider.characterCausingDamage = characterWithWeapon;
        meleeDamageCollider.physicalDamage = weapon.physicalDamage;
        meleeDamageCollider.fireDamage = weapon.fireDamage;
        meleeDamageCollider.lightningDamage = weapon.lightningDamage;
        meleeDamageCollider.poisonDamage = weapon.iceDamage;
        meleeDamageCollider.iceDamage = weapon.poisonDamage;
        meleeDamageCollider.shadowDamage = weapon.decayDamage;
        meleeDamageCollider.decayDamage = weapon.shadowDamage;

        meleeDamageCollider.light_Attack_01_DamageModifier = weapon.light_Attack_01_DamageModifier;
        meleeDamageCollider.light_Attack_02_DamageModifier = weapon.light_Attack_02_DamageModifier;
        meleeDamageCollider.heavy_Attack_01_DamageModifier = weapon.heavy_Attack_01_DamageModifier;
        meleeDamageCollider.heavy_Attack_02_DamageModifier = weapon.heavy_Attack_02_DamageModifier;
        meleeDamageCollider.charge_Attack_01_DamageModifier = weapon.charge_Attack_01_DamageModifier;
        meleeDamageCollider.charge_Attack_02_DamageModifier = weapon.charge_Attack_02_DamageModifier;
        meleeDamageCollider.light_Run_Attack_01_DamageModifier = weapon.light_Run_Attack_01_DamageModifier;
        meleeDamageCollider.light_Roll_Attack_01_DamageModifier = weapon.light_Roll_Attack_01_DamageModifier;
        meleeDamageCollider.light_BackStep_Attack_01_DamageModifier = weapon.light_BackStep_Attack_01_DamageModifier;

    }
}
