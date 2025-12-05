using UnityEngine;

public class EnemyEquipmentManager : CharacterEquipmentManager
{
    EnemyCharacterManager enemy;

    public WeaponModelInstantiateSlot rightHandSlot;
    public WeaponModelInstantiateSlot leftHandSlot;

    [SerializeField] WeaponManager rightWeaponManager;
    [SerializeField] WeaponManager leftWeaponManager;

    [SerializeField] WeaponItem currentRightHandWeapon;
    [SerializeField] WeaponItem currentLeftHandWeapon;

    public GameObject rightHandWeaponModel;
    public GameObject leftHandWeaponModel;

    protected override void Awake()
    {
        base.Awake();
        enemy = GetComponent<EnemyCharacterManager>();
        InitializeWeaponSlots();
    }
    protected override void Start()
    {
        base.Start();
        LoadWeaponsOnBothHands();
    }

    private void InitializeWeaponSlots()
    {
        WeaponModelInstantiateSlot[] weaponSlots = GetComponentsInChildren<WeaponModelInstantiateSlot>();
        foreach (var weaponSlot in weaponSlots)
        {
            if (weaponSlot.weaponSlot == WeaponModelSlot.RightHand)
            {
                rightHandSlot = weaponSlot;
            }
            else if (weaponSlot.weaponSlot == WeaponModelSlot.LeftHand)
            {
                leftHandSlot = weaponSlot;
            }
        }
    }
    public void LoadWeaponsOnBothHands()
    {
        LoadLeftWeapon();
        LoadRightWeapon();
    }
    public void LoadRightWeapon()
    {
        if (currentRightHandWeapon != null)
        {
            //remove old weapon
            rightHandSlot.UnloadWeapon();

            //bring in new weapon 
            rightHandWeaponModel = Instantiate(currentRightHandWeapon.weaponModel);
            rightHandSlot.LoadWeapon(rightHandWeaponModel);
            rightWeaponManager = rightHandWeaponModel.GetComponent<WeaponManager>();
            rightWeaponManager.SetWeaponDamage(enemy, currentRightHandWeapon);
            //enemy.enemyAnimationManager.UpdateAnimatorController(currentRightHandWeapon.weaponOverrideAnimator);
        }
    }
    public void LoadLeftWeapon()
    {
        if (currentLeftHandWeapon != null)
        {
            //remove old weapon
            leftHandSlot.UnloadWeapon();

            //bring in new weapon 
            leftHandWeaponModel = Instantiate(currentLeftHandWeapon.weaponModel);
            leftHandSlot.LoadWeapon(leftHandWeaponModel);
            leftWeaponManager = leftHandWeaponModel.GetComponent<WeaponManager>();
            leftWeaponManager.SetWeaponDamage(enemy, currentLeftHandWeapon);
            //enemy.enemyAnimationManager.UpdateAnimatorController(currentLeftHandWeapon.weaponOverrideAnimator);
        }
    }
    public MeleeWeaponDamageCollider GetLeftWeaponCollider()
    {
        return leftWeaponManager.GetComponentInChildren<MeleeWeaponDamageCollider>();
    }
    public MeleeWeaponDamageCollider GetRightWeaponCollider()
    {
        return rightWeaponManager.GetComponentInChildren<MeleeWeaponDamageCollider>();
    }
}
