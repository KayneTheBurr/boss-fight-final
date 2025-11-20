using UnityEngine;

[CreateAssetMenu(menuName = "Character Actions/Weapon Actions")]
public class WeaponItemAction : ScriptableObject
{
    public string actionID;

    public virtual void AttemptToPerformAction(PlayerManager playerPerformingAction, WeaponItem weaponPerformingAction)
    {
        //Debug.Log("Do the weapon action with" + weaponPerformingAction);
        //we should always keep track of what weapon a player is using 
        if (playerPerformingAction)
        {
            playerPerformingAction.playerCombatManager.currentWeaponBeingUsedID.SetString(weaponPerformingAction.itemID);
            //Debug.Log(weaponPerformingAction.itemID);
        }

    }
}
