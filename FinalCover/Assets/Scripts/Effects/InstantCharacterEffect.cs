using UnityEngine;

[CreateAssetMenu(menuName = "Character Effect/Instant Effect")]
public class InstantCharacterEffect : ScriptableObject
{
    [Header("Effect ID")]
    public string instantEffectID;



    public virtual void ProcessEffect(CharacterManager character)
    {

    }
}
