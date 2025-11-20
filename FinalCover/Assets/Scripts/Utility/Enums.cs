using UnityEngine;

public class Enums : MonoBehaviour
{}
public enum EnemyAttackType { Melee, Ranged}
public enum RotationMode
{
    SoulsLike,  //souls like direction depends on camera direction
    DiabloLike      //poe/diablo style with fixed camera
}
public enum CharacterGroup
{
    Friendly,
    Enemy
}
public enum WeaponClass
{
    Fists,
    Katana, 
    Greatsword,
    Bow
}
public enum AttackType 
{
    LightAttack01,
    LightAttack02,
    LightAttack03,
    LightAttack04,
    LightAttack05,
    HeavyAttack01,
    HeavyAttack02,
    HeavyAttack03,
    HeavyAttack04,
    HeavyAttack05,
    ChargeAttack01,
    ChargeAttack02,
    ChargeAttack03,
    ChargeAttack04,
    ChargeAttack05,
    LightRunningAttack01,
    LightRollingAttack01,
    LightBackStepAttack01
}

public enum WeaponModelSlot
{
    RightHand,
    LeftHand//,
    //RightHip,
    //LeftHip,
    //Back
}