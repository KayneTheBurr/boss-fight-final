using System.Linq;
using UnityEngine;

public class WorldActionManager : MonoBehaviour
{
    public static WorldActionManager instance;

    [Header("Weapon Item Actions")]
    public WeaponItemAction[] weaponItemActions;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        //DontDestroyOnLoad(gameObject);

        //DOES NOT WORK WITH STRINGS, USE CSV IMPORTER
        //for (int i = 0; i < weaponItemActions.Length; i++)
        //{
            //weaponItemActions[i].actionID = i;
        //}
    }

    public WeaponItemAction GetWeaponItemActionByID(string ID)
    {
        if (string.IsNullOrWhiteSpace(ID)) return null;
        
        return weaponItemActions.FirstOrDefault(action => action.actionID == ID);
    }
}
