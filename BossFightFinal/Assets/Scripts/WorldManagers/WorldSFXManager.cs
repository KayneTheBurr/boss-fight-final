using UnityEngine;

public class WorldSFXManager : MonoBehaviour
{
    public static WorldSFXManager instance;

    [Header("Action SFX")]
    public AudioClip rollSFX;

    [Header("Damage Sounds")]
    public AudioClip[] physicalDamageSFX;

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
    }
    private void Start()
    {
        //DontDestroyOnLoad(gameObject);
    }

    public AudioClip ChooseRandomSFXFromArray(AudioClip[] array)
    {
        if (array.Length == 0) return null;

        //given an array of sfx, choose one randomly and return it 
        int index = Random.Range(0, array.Length);
        return array[index];
    }

}
