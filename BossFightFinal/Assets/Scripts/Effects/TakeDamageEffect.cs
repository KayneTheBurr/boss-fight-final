using UnityEngine;

[CreateAssetMenu(menuName = "Character Effect/Instant Effect/Take Damage")]
public class TakeDamageEffect : InstantCharacterEffect
{
    [Header("Character Causing Damage")]
    public CharacterManager characterCausingDamage; //if dmg is done by another character attack, store who they are here 

    [Header("Damage")]
    public float physicalDamage = 0; //break down into sub types (standard, slash, pierce, strike)
    public float fireDamage = 0;
    public float lightningDamage = 0;
    public float iceDamage = 0;
    public float poisonDamage = 0;
    public float decayDamage = 0;
    public float shadowDamage = 0;

    //build up effects added as well here 

    [Header("Final Damage")] //damage character actually takes after all effects have been processed
    private float finalDamage = 0;

    [Header("Poise")]
    public float poiseDamage = 0;
    public bool poiseIsBroken = false; //false by defualt, play "stun" animation if poise is actually broken by effect

    [Header("Animation")]
    public bool playDamageAnimation = true;
    public bool manuallySelectDamageAnimation = false;
    public string damageAnimation;

    [Header("Sound Effects")]
    public bool willPlayDamageSFX = true;
    public AudioClip elementalDamageSFX; //additive sfx if damage type has an element attached to it additionally

    [Header("Direction Damage Taken From")]
    //determine where hit from for which animation to play, where blood sprays etc 
    public float angleHitFrom;
    public Vector3 contactPoint; //where blood fx plays from

    public override void ProcessEffect(CharacterManager character)
    {
        base.ProcessEffect(character);

        // if the character is dead, dont process any additional damage effects since they are already dead
        if (character.isDead) return;

        //check for invulnerability window
        if (character.characterCombatManager.isInvulnerable) return;

        CalculateDamage(character);

        //check which direction damage came from and play damage animation
        //PlayDirectionalBasedDamageAnimation(character);

        //check for build up effect (poison bleed etc)

        //play damage sound fx
        //PlayDamageSFX(character);

        //play damge vfx (blood etc)
        //PlayDamageVFX(character);

        //if characcter is AI enemey, check for who just attacked them 

    }


    private void CalculateDamage(CharacterManager character)
    {
        if (!character) return;

        if (characterCausingDamage != null) //if damage is doen by another character and not the environment
        {
            //check the characters modifiers and modify the base damage

        }
        //check character for flat damage reduction, subtract them from the specific type of damage
        iceDamage = Mathf.Max(0, iceDamage - character.characterStatManager.flatIceDefense);

        //check for character armor absorbtions, subtract percentage from damage

        //add all damage types together and apply final damage
        finalDamage = (physicalDamage + fireDamage + lightningDamage + poisonDamage + iceDamage + decayDamage + shadowDamage);
        if (finalDamage <= 0)
        {
            finalDamage = 1;
        }

        Debug.Log("Dealt " + finalDamage + " damage!");

        character.characterStatManager.currentHealth.SetFloat(character.characterStatManager.currentHealth.GetFloat()-finalDamage);
        
        //calculate poise/stagger damage to determine if character will be stunned and play damaged animation or not 
    }
}
