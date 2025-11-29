using UnityEngine;
using UnityEngine.Rendering;

[System.Serializable]
public class CharacterSaveData
{
    [Header("Scene Index")]
    public int sceneIndexNumber = 1;


    [Header("Character Name")]
    public string characterName = "Character";

    [Header("Time Played")]
    public float timePlayedSec = 0;

    //cannot save vector3, only basic bar types 
    [Header("Position")]
    public float xPos;
    public float yPos;
    public float zPos;

    [Header("Attributes")]
    public int heart;
    public int strength;
    public int agility;
    public int arcana;
    public int essence;
    public int cunning;
    public int omen;

    [Header("Resources")]
    public float currentHealth;
    public float currentStamina;

    [Header("Bosses")]
    public SerializedDictionary<string, bool> bossesAwakened; //string is boss ID, bool is boss room entered/boss awakened 
    public SerializedDictionary<string, bool> bossesDefeated; //string is boss ID, bool is boss defeated or not 

    public CharacterSaveData()
    {
        bossesAwakened = new SerializedDictionary<string, bool>();
        bossesDefeated = new SerializedDictionary<string, bool>();
    }
}
