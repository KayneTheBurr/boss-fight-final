using UnityEngine;

[CreateAssetMenu(menuName = "Character Effect/Instant Effect/Take Mana Damage")]
public class TakeManaDamageEffect : InstantCharacterEffect
{
    public float manaDamage;

    public override void ProcessEffect(CharacterManager character)
    {
        CalculateStaminaDamage(character);
    }

    private void CalculateStaminaDamage(CharacterManager character)
    {
        if (character)
        {
            character.characterStatManager.currentStamina.SetFloat(
                character.characterStatManager.currentStamina.GetFloat() - manaDamage);

            Debug.Log("Character takes" + manaDamage + "stamina damage");

        }
    }
}
