using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WorldItemDataBase : MonoBehaviour
{
    public static WorldItemDataBase instance;

    [SerializeField] List<WeaponItem> weapons = new List<WeaponItem>();
    private List<Item> items = new List<Item>();

    public WeaponItem unarmedWeapon;

    
    public List<WeaponEntry> weaponEntries = new();
    public List<ItemEntry> itemEntries = new();

    [System.Serializable]
    public class WeaponEntry
    {
        public string weaponID;
        public WeaponItem weaponItem;
    }

    public class ItemEntry
    {
        public string itemID;
        public Item item;
    }

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

        foreach (var weapon in weapons)
        {
            items.Add(weapon);
        }
        //for (int i = 0; i < items.Count; i++) //not used if using string based IDs
        //{
        //    items[i].itemID = i;
        //}

        //CREATE THE INDEX OF ALL ITEM TYPES HERE
        //WEAPONS
        //ARMOR
        //ITEMS, etc

    }

    private void Start()
    {
        //DontDestroyOnLoad(gameObject);
    }

    //public WeaponItem GetWeaponByID(int ID) this needs to be updated is using string based IDs
    //{
    //    return weapons.IndexOf("string");// FirstOrDefault(weapon => weapon.itemID == ID);
    //}

    public WeaponItem GetWeaponByID(string ID)
    {
        if (string.IsNullOrWhiteSpace(ID)) return null;
        return weaponEntries.Find(e => e.weaponID == ID)?.weaponItem;
    }
}
