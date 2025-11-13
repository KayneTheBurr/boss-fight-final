using UnityEngine;

public class PlayerEquipmentManager : CharacterEquipmentManager
{
    PlayerManager player;

    public WeaponModelInstantiateSlot rightHandSlot;
    public WeaponModelInstantiateSlot leftHandSlot;

    [SerializeField] WeaponManager rightWeaponManager;
    [SerializeField] WeaponManager leftWeaponManager;

    public GameObject rightHandWeaponModel;
    public GameObject leftHandWeaponModel;

    protected override void Awake()
    {
        base.Awake();
        player = GetComponent<PlayerManager>();

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
    //Right Hand
    public void LoadRightWeapon()
    {
        if (player.playerInventoryManager.currentRightHandWeapon != null)
        {
            //remove old weapon
            rightHandSlot.UnloadWeapon();

            //bring in new weapon 
            rightHandWeaponModel = Instantiate(player.playerInventoryManager.currentRightHandWeapon.weaponModel);
            rightHandSlot.LoadWeapon(rightHandWeaponModel);
            rightWeaponManager = rightHandWeaponModel.GetComponent<WeaponManager>();
            rightWeaponManager.SetWeaponDamage(player, player.playerInventoryManager.currentRightHandWeapon);
            player.playerAnimatorManager.UpdateAnimatorController(player.playerInventoryManager.currentRightHandWeapon.weaponOverrideAnimator);
            

        }
    }
    public void SwitchRightWeapon()
    {
        if (!player) return;

        player.playerAnimatorManager.PlayTargetActionAnimation("Swap_Right_weapon_01", false, false, true, true);
        //if we have 2 weapons equiped, never swap to unarmed
        //if he have one weapon equipped, swap to unarmed and back, not through both unarmed slots 

        WeaponItem selectedWeapon = null;

        //disable 2 handing weapon if we are doing that 

        //add one to weapon index to select next weapon
        player.playerInventoryManager.rightHandWeaponIndex += 1;

        //check if out weapon index is out of bounds, if so reset to first weapon slot (0)
        if (player.playerInventoryManager.rightHandWeaponIndex < 0 || player.playerInventoryManager.rightHandWeaponIndex > 2)
        {
            player.playerInventoryManager.rightHandWeaponIndex = 0;

            //check if we are holding more than one weapon
            float weaponCount = 0;
            WeaponItem firstWeapon = null;
            int firstWeaponPos = 0;
            for (int i = 0; i < player.playerInventoryManager.weaponsInRightHandSlots.Length; i++)
            {
                if (player.playerInventoryManager.weaponsInRightHandSlots[i].itemID != WorldItemDataBase.instance.unarmedWeapon.itemID)
                {
                    weaponCount++;

                    if (firstWeapon == null)
                    {
                        firstWeapon = player.playerInventoryManager.weaponsInRightHandSlots[i];
                        firstWeaponPos = i;
                    }
                }
            }
            if (weaponCount <= 1)
            {
                player.playerInventoryManager.rightHandWeaponIndex = -1;
                selectedWeapon = WorldItemDataBase.instance.unarmedWeapon;
                player.playerCombatManager.currentRightWeaponID.SetString(selectedWeapon.itemID);
            }
            else
            {
                player.playerInventoryManager.rightHandWeaponIndex = firstWeaponPos;
                player.playerCombatManager.currentRightWeaponID.SetString(firstWeapon.itemID);
            }
            return;
        }
        foreach (WeaponItem weapon in player.playerInventoryManager.weaponsInRightHandSlots)
        {
            //if the next weapon is not unarmed weapon
            if (player.playerInventoryManager.weaponsInRightHandSlots[player.playerInventoryManager.rightHandWeaponIndex].itemID !=
                WorldItemDataBase.instance.unarmedWeapon.itemID)
            {
                //assign network weapon so it switches for all connected
                selectedWeapon = player.playerInventoryManager.weaponsInRightHandSlots[player.playerInventoryManager.rightHandWeaponIndex];
                player.playerCombatManager.currentRightWeaponID.SetString(
                    player.playerInventoryManager.weaponsInRightHandSlots[player.playerInventoryManager.rightHandWeaponIndex].itemID);
                return;
            }
        }
        if (selectedWeapon == null && player.playerInventoryManager.rightHandWeaponIndex <= 2)
        {

            SwitchRightWeapon();
        }

    }

    //Left Hand
    public void LoadLeftWeapon()
    {
        if (player.playerInventoryManager.currentLeftHandWeapon != null)
        {
            //remove old weapon
            leftHandSlot.UnloadWeapon();

            //bring in new weapon
            leftHandWeaponModel = Instantiate(player.playerInventoryManager.currentLeftHandWeapon.weaponModel);
            leftHandSlot.LoadWeapon(leftHandWeaponModel);
            leftWeaponManager = leftHandWeaponModel.GetComponent<WeaponManager>();
            leftWeaponManager.SetWeaponDamage(player, player.playerInventoryManager.currentLeftHandWeapon);
        }
    }

    public void SwitchLeftWeapon()
    {
        if (!player) return;

        player.playerAnimatorManager.PlayTargetActionAnimation("Swap_Left_weapon_01", false, false, true, true);
        //if we have 2 weapons equiped, never swap to unarmed
        //if he have one weapon equipped, swap to unarmed and back, not through both unarmed slots 

        WeaponItem selectedWeapon = null;

        //disable 2 handing weapon if we are doing that 

        //add one to weapon index to select next weapon
        player.playerInventoryManager.leftHandWeaponIndex += 1;

        //check if out weapon index is out of bounds, if so reset to first weapon slot (0)
        if (player.playerInventoryManager.leftHandWeaponIndex < 0 || player.playerInventoryManager.leftHandWeaponIndex > 2)
        {
            player.playerInventoryManager.leftHandWeaponIndex = 0;

            //check if we are holding more than one weapon
            float weaponCount = 0;
            WeaponItem firstWeapon = null;
            int firstWeaponPos = 0;
            for (int i = 0; i < player.playerInventoryManager.weaponsInLeftHandSlots.Length; i++)
            {
                if (player.playerInventoryManager.weaponsInLeftHandSlots[i].itemID != WorldItemDataBase.instance.unarmedWeapon.itemID)
                {
                    weaponCount++;

                    if (firstWeapon == null)
                    {
                        firstWeapon = player.playerInventoryManager.weaponsInLeftHandSlots[i];
                        firstWeaponPos = i;
                    }
                }
            }
            if (weaponCount <= 1)
            {
                player.playerInventoryManager.leftHandWeaponIndex = -1;
                selectedWeapon = WorldItemDataBase.instance.unarmedWeapon;
                player.playerCombatManager.currentLeftWeaponID.SetString(selectedWeapon.itemID);
            }
            else
            {
                player.playerInventoryManager.leftHandWeaponIndex = firstWeaponPos;
                player.playerCombatManager.currentLeftWeaponID.SetString(firstWeapon.itemID);
            }
            return;
        }
        foreach (WeaponItem weapon in player.playerInventoryManager.weaponsInLeftHandSlots)
        {
            //if the next weapon is not unarmed weapon
            if (player.playerInventoryManager.weaponsInLeftHandSlots[player.playerInventoryManager.leftHandWeaponIndex].itemID !=
                WorldItemDataBase.instance.unarmedWeapon.itemID)
            {
                //assign network weapon so it switches for all connected
                selectedWeapon = player.playerInventoryManager.weaponsInLeftHandSlots[player.playerInventoryManager.leftHandWeaponIndex];
                player.playerCombatManager.currentLeftWeaponID.SetString(
                    player.playerInventoryManager.weaponsInLeftHandSlots[player.playerInventoryManager.leftHandWeaponIndex].itemID);
                return;
            }
        }
        if (selectedWeapon == null && player.playerInventoryManager.leftHandWeaponIndex <= 2)
        {

            SwitchLeftWeapon();
        }
    }

    //Damage Colliders
    public void OpenDamageCollider()
    {
        //use right weapon damage collider 
        if (player.playerCombatManager.isUsingRightHand.GetBool())
        {
            rightWeaponManager.meleeDamageCollider.EnableDamageCollider();
            player.characterSFXManager.PlaySoundFX(WorldSFXManager.instance.
                ChooseRandomSFXFromArray(player.playerInventoryManager.currentRightHandWeapon.whooshes));
        }
        //use elft weapon
        else if (player.playerCombatManager.isUsingLeftHand.GetBool())
        {
            leftWeaponManager.meleeDamageCollider.EnableDamageCollider();
            player.characterSFXManager.PlaySoundFX(WorldSFXManager.instance.
                ChooseRandomSFXFromArray(player.playerInventoryManager.currentLeftHandWeapon.whooshes));
        }
        //play damage swoosh sfx
    }

    public void CloseDamageCollider()
    {
        //use right weapon damage collider 
        if (player.playerCombatManager.isUsingRightHand.GetBool())
        {
            rightWeaponManager.meleeDamageCollider.DisableDamageCollider();
        }
        //use elft weapon
        else if (player.playerCombatManager.isUsingLeftHand.GetBool())
        {
            leftWeaponManager.meleeDamageCollider.DisableDamageCollider();
        }

    }
}
