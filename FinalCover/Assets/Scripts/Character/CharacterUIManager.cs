using UnityEngine;

public class CharacterUIManager : MonoBehaviour
{
    [Header("UI")]
    //public UI_Character_HP_Bar characterHPBar;
    public bool hasFloatingHPBar = false;

    public void OnHPChanged(float oldValue, float newValue)
    {
        //characterHPBar.oldHPValue = oldValue;
        //characterHPBar.SetStat(newValue);
    }
}
