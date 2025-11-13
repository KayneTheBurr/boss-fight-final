using System.Collections.Generic;
using UnityEngine;

public class WorldCharacterEffectsManager : MonoBehaviour
{
    public static WorldCharacterEffectsManager instance;

    [Header("Damage")]
    public TakeDamageEffect takeDamageEffect;
    public GameObject bloodSplatterVFX;

    [SerializeField] List<InstantCharacterEffect> instantEffects;

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
        //GenerateEffectIDs();
    }
    private void Start()
    {
        //DontDestroyOnLoad(gameObject);
    }
}
