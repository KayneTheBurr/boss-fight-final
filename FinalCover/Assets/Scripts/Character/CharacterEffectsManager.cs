using UnityEngine;

public class CharacterEffectsManager : MonoBehaviour
{
    CharacterManager character;

    [Header("VFX")]
    [SerializeField] GameObject bloodSplatterVFX;

    protected virtual void Awake()
    {
        character = GetComponent<CharacterManager>();
    }


    //over time effect (poison, buff) effect processed over a period of time
    //static on/off effects (armor/jewelery effects)


    //instnat effects (heal, dmage) one and done effects
    public virtual void ProcessInstantEffects(InstantCharacterEffect effect)
    {
        //take in an effect / do what the effect causes
        effect.ProcessEffect(character);
    }
}
