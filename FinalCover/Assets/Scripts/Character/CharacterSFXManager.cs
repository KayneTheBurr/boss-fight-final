using UnityEngine;

public class CharacterSFXManager : MonoBehaviour
{
    private AudioSource audioSource;

    [Header("Damage Grunts")]
    [SerializeField] protected AudioClip[] damageGrunts;
    [SerializeField] protected AudioClip[] attackGrunts;

    protected virtual void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayDodgeSFX()
    {
        //Debug.Log("dodge sfx");
        if (WorldSFXManager.instance.rollSFX == null) return;
        audioSource.PlayOneShot(WorldSFXManager.instance.rollSFX);
    }
    public void PlaySoundFX(AudioClip soundFX, float volume = 1, bool randomizePitch = true, float pitchRandomAmount = 0.1f)
    {
        audioSource.PlayOneShot(soundFX, volume);

        //reset pitch
        audioSource.pitch = 1;

        if (randomizePitch)
        {
            audioSource.pitch += Random.Range(-pitchRandomAmount, pitchRandomAmount);
        }
    }
    public virtual void PlayDamageGrunt()
    {
        PlaySoundFX(WorldSFXManager.instance.ChooseRandomSFXFromArray(damageGrunts));
    }

    public virtual void PlayAttackGrunt()
    {

    }

}
