using UnityEngine;

public class PlayerCombatManager : CharacterCombatManager
{
    PlayerManager player;

    public WeaponItem currentWeaponBeingUsed;
    public ObservableVariable currentWeaponBeingUsedID = new ObservableVariable("");
    public ObservableVariable currentRightWeaponID = new ObservableVariable("");
    public ObservableVariable currentLeftWeaponID = new ObservableVariable("");
    public ObservableVariable isUsingRightHand = new ObservableVariable(false);
    public ObservableVariable isUsingLeftHand = new ObservableVariable(false);

    //public WeaponItem currentWeaponBeingUsed;
    public bool canComboWithMainHandWeapon = false;

    protected override void Awake()
    {
        base.Awake();
        player = GetComponent<PlayerManager>();
    }
    protected override void OnEnable()
    {
        base.OnEnable();
        currentRightWeaponID.OnStringChanged += OnCurrentRightHandWeaponIDChange;
        currentLeftWeaponID.OnStringChanged += OnCurrentLeftHandWeaponIDChange;
        currentWeaponBeingUsedID.OnStringChanged += OnCurrentWeaponBeingUsedIDChange;
    }
    protected override void OnDisable()
    {
        base.OnDisable();
        currentRightWeaponID.OnStringChanged -= OnCurrentRightHandWeaponIDChange;
        currentLeftWeaponID.OnStringChanged -= OnCurrentLeftHandWeaponIDChange;
        currentWeaponBeingUsedID.OnStringChanged -= OnCurrentWeaponBeingUsedIDChange;
    }

    public void PerformWeaponBasedAction(WeaponItemAction weaponAction, WeaponItem weaponPerformingAction)
    {
        if (player)
        {
            //perform the action here
            weaponAction.AttemptToPerformAction(player, weaponPerformingAction);
        }
    }

    public override void EnableCanDoCombo()
    {
        base.EnableCanDoCombo();
        //Debug.Log("can combo");
        canComboWithMainHandWeapon = true;
    }
    public override void DisableCanDoCombo()
    {
        base.DisableCanDoCombo();
        canComboWithMainHandWeapon = false;
    }
    public void DrainStaminaBasedOnAttack()
    {
        if (!player) return;
        
        if (currentWeaponBeingUsed == null) return;
        
        float staminaDrained = 0f;

        switch (currentAttackType)
        {
            case AttackType.LightAttack01:
                staminaDrained = currentWeaponBeingUsed.baseStaminaCost * currentWeaponBeingUsed.lightAttackStaminaCostModifier;
                break;
            case AttackType.LightAttack02:
                staminaDrained = currentWeaponBeingUsed.baseStaminaCost * currentWeaponBeingUsed.lightAttackStaminaCostModifier;
                break;
            case AttackType.LightAttack03:
                staminaDrained = currentWeaponBeingUsed.baseStaminaCost * currentWeaponBeingUsed.lightAttackStaminaCostModifier;
                break;
            case AttackType.LightAttack04:
                staminaDrained = currentWeaponBeingUsed.baseStaminaCost * currentWeaponBeingUsed.lightAttackStaminaCostModifier;
                break;
            case AttackType.LightAttack05:
                staminaDrained = currentWeaponBeingUsed.baseStaminaCost * currentWeaponBeingUsed.lightAttackStaminaCostModifier;
                break;
            case AttackType.HeavyAttack01:
                staminaDrained = currentWeaponBeingUsed.baseStaminaCost * currentWeaponBeingUsed.heavyAttackStaminaCostModifier;
                break;
            case AttackType.HeavyAttack02:
                staminaDrained = currentWeaponBeingUsed.baseStaminaCost * currentWeaponBeingUsed.heavyAttackStaminaCostModifier;
                break;
            case AttackType.HeavyAttack03:
                staminaDrained = currentWeaponBeingUsed.baseStaminaCost * currentWeaponBeingUsed.heavyAttackStaminaCostModifier;
                break;
            case AttackType.HeavyAttack04:
                staminaDrained = currentWeaponBeingUsed.baseStaminaCost * currentWeaponBeingUsed.heavyAttackStaminaCostModifier;
                break;
            case AttackType.HeavyAttack05:
                staminaDrained = currentWeaponBeingUsed.baseStaminaCost * currentWeaponBeingUsed.heavyAttackStaminaCostModifier;
                break;
            case AttackType.ChargeAttack01:
                staminaDrained = currentWeaponBeingUsed.baseStaminaCost * currentWeaponBeingUsed.chargeAttackStaminaCostModifier;
                break;
            case AttackType.ChargeAttack02:
                staminaDrained = currentWeaponBeingUsed.baseStaminaCost * currentWeaponBeingUsed.chargeAttackStaminaCostModifier;
                break;
            case AttackType.ChargeAttack03:
                staminaDrained = currentWeaponBeingUsed.baseStaminaCost * currentWeaponBeingUsed.chargeAttackStaminaCostModifier;
                break;
            case AttackType.ChargeAttack04:
                staminaDrained = currentWeaponBeingUsed.baseStaminaCost * currentWeaponBeingUsed.chargeAttackStaminaCostModifier;
                break;
            case AttackType.ChargeAttack05:
                staminaDrained = currentWeaponBeingUsed.baseStaminaCost * currentWeaponBeingUsed.chargeAttackStaminaCostModifier;
                break;
            case AttackType.LightRunningAttack01:
                staminaDrained = currentWeaponBeingUsed.baseStaminaCost * currentWeaponBeingUsed.lightRunAttackStaminaCostModifier;
                break;
            case AttackType.LightRollingAttack01:
                staminaDrained = currentWeaponBeingUsed.baseStaminaCost * currentWeaponBeingUsed.lightRollAttackStaminaCostModifier;
                break;
            case AttackType.LightBackStepAttack01:
                staminaDrained = currentWeaponBeingUsed.baseStaminaCost * currentWeaponBeingUsed.lightBackStepAttackStaminaCostModifier;
                break;
            default:
                break;
        }

        //Debug.Log($"Used {staminaDrained} stamina on attack");

        player.playerStatsManager.currentStamina.SetFloat(
            player.playerStatsManager.currentStamina.GetFloat() - staminaDrained);
    }

    public override void SetTarget(CharacterManager newTarget)
    {
        base.SetTarget(newTarget);

        if (player)
        {
            //PlayerCamera.instance.SetNewTargetAsLookAtTarget(newTarget);
        }
    }

    public void SetCharacterActionHand(bool rightHandedAction)
    {
        if (rightHandedAction)
        {
            isUsingRightHand.SetBool(true);
            isUsingLeftHand.SetBool(false);
        }
        else
        {
            isUsingRightHand.SetBool(false);
            isUsingLeftHand.SetBool(true);
        }
    }
    public void OnCurrentRightHandWeaponIDChange(string oldID, string newID)
    {
        WeaponItem newWeapon = Instantiate(WorldItemDataBase.instance.GetWeaponByID(newID));
        player.playerInventoryManager.currentRightHandWeapon = newWeapon;
        player.playerEquipmentManager.LoadRightWeapon();

        if (player)
        {
            PlayerUIManager.instance.playerHUDManager.SetRightWeaponQuickSlotIcon(newID);
        }
    }
    public void OnCurrentLeftHandWeaponIDChange(string oldID, string newID)
    {
        WeaponItem newWeapon = Instantiate(WorldItemDataBase.instance.GetWeaponByID(newID));
        player.playerInventoryManager.currentLeftHandWeapon = newWeapon;
        player.playerEquipmentManager.LoadLeftWeapon();
        if (player)
        {
            PlayerUIManager.instance.playerHUDManager.SetLeftWeaponQuickSlotIcon(newID);
        }
    }
    public void OnCurrentWeaponBeingUsedIDChange(string oldID, string newID)
    {
        //Debug.Log(newID);
        WeaponItem newWeapon = Instantiate(WorldItemDataBase.instance.GetWeaponByID(newID));
        player.playerCombatManager.currentWeaponBeingUsed = newWeapon;

    }

    public void PerformWeaponAction(string actionID, string weaponID)
    {
        WeaponItemAction weaponAction = WorldActionManager.instance.GetWeaponItemActionByID(actionID);

        if (weaponAction != null)
        {
            weaponAction.AttemptToPerformAction(player, WorldItemDataBase.instance.GetWeaponByID(weaponID));
        }
        else
        {
            Debug.Log("Action is null, ERROR ERROR");
        }
    }
}
