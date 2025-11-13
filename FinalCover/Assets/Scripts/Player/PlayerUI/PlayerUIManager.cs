using UnityEngine;

public class PlayerUIManager : MonoBehaviour
{
    public static PlayerUIManager instance;

    [SerializeField] public PlayerHUDManager playerHUDManager;
    [HideInInspector] public PlayerUIPopupManager playerUIPopupManager;

    private void Awake()
    {
        //one at a time 
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        //playerHUDManager = GetComponentInChildren<PlayerHUDManager>();
        playerUIPopupManager = GetComponentInChildren<PlayerUIPopupManager>();
    }
    private void Start()
    {
        //DontDestroyOnLoad(gameObject);
    }
}
