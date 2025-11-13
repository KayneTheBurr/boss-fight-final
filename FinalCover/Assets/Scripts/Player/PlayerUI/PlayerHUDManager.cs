using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHUDManager : MonoBehaviour
{
    [Header("Stat bars")]
    [SerializeField] UI_StatBar healthBar;
    [SerializeField] UI_StatBar staminaBar;

    [Header("Quick Slots")]
    [SerializeField] Image rightWeaponQuickSlotIcon;
    [SerializeField] Image leftWeaponQuickSlotIcon;

    [Header("Boss Panel")]
    public GameObject bossPanel;
    public TMP_Text bossNameLabel;
    public TMP_Text bossNameLabelShadow;
    public Slider bossHPBar;

    public void RefreshHUD()
    {
        healthBar.gameObject.SetActive(false);
        healthBar.gameObject.SetActive(true);
        staminaBar.gameObject.SetActive(false);
        staminaBar.gameObject.SetActive(true);
    }

    public void SetNewHealthValue(float oldHealth, float newHealth)
    {
        
        healthBar.SetStat(newHealth);
    }
    public void SetMaxHealthValue(float maxHealth)
    {
        healthBar.SetMaxStat(maxHealth);
    }
    public void SetNewStaminaValue(float oldStamina, float newStamina)
    {
        
        staminaBar.SetStat(newStamina);
    }
    public void SetMaxStaminaValue(float maxStamina)
    {
        staminaBar.SetMaxStat(maxStamina);
    }
    public void SetNewManaValue(float oldMana, float newMana)
    {
        Debug.Log("set stamina");
        staminaBar.SetStat(newMana);
    }
    public void SetMaxManaValue(float maxMana)
    {
        staminaBar.SetMaxStat(maxMana);
    }

    public void SetRightWeaponQuickSlotIcon(string weaponID)
    {
        WeaponItem weapon = WorldItemDataBase.instance.GetWeaponByID(weaponID);

        if (weapon == null)
        {
            Debug.Log("Item is null");
            rightWeaponQuickSlotIcon.enabled = false;
            rightWeaponQuickSlotIcon.sprite = null;
            return;
        }
        if (weapon.itemIcon == null)
        {
            Debug.Log("Item has no icon");
            rightWeaponQuickSlotIcon.enabled = false;
            rightWeaponQuickSlotIcon.sprite = null;
            return;
        }
        //check to see if you meet the weapons requirements for the weapon stats

        rightWeaponQuickSlotIcon.sprite = weapon.itemIcon;
        rightWeaponQuickSlotIcon.enabled = true;


    }
    public void SetLeftWeaponQuickSlotIcon(string weaponID)
    {
        WeaponItem weapon = WorldItemDataBase.instance.GetWeaponByID(weaponID);

        if (weapon == null)
        {
            Debug.Log("Item is null");
            leftWeaponQuickSlotIcon.enabled = false;
            leftWeaponQuickSlotIcon.sprite = null;
            return;
        }
        if (weapon.itemIcon == null)
        {
            Debug.Log("Item has no icon");
            leftWeaponQuickSlotIcon.enabled = false;
            leftWeaponQuickSlotIcon.sprite = null;
            return;
        }
        //check to see if you meet the weapons requirements for the weapon stats

        leftWeaponQuickSlotIcon.sprite = weapon.itemIcon;
        leftWeaponQuickSlotIcon.enabled = true;
    }
}
