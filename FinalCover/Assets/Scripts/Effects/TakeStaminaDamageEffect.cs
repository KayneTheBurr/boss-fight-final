using UnityEngine;

[CreateAssetMenu(menuName = "Character Effect/Instant Effect/Take Stamina Damage")]
public class TakeStaminaDamageEffect : InstantCharacterEffect
{
    public float staminaDamage;

    public override void ProcessEffect(CharacterManager character)
    {
        CalculateStaminaDamage(character);
    }

    private void CalculateStaminaDamage(CharacterManager character)
    {
        if (character)
        {
            character.characterStatManager.currentStamina.SetFloat(
                character.characterStatManager.currentStamina.GetFloat()-staminaDamage);
            
            Debug.Log("Character takes" + staminaDamage + "stamina damage");

        }
    }
}
