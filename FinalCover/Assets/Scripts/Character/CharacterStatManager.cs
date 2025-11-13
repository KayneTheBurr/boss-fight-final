using UnityEngine;
using UnityEngine.TextCore.Text;

public class CharacterStatManager : MonoBehaviour
{
    CharacterManager character;

    [Header("Core Resources")]
    public ObservableVariable maxHealth = new ObservableVariable(100);
    public ObservableVariable currentHealth = new ObservableVariable(100f);
    public ObservableVariable maxStamina = new ObservableVariable(100);
    public ObservableVariable currentStamina = new ObservableVariable(100f);
    public ObservableVariable maxMana = new ObservableVariable(100);
    public ObservableVariable currentMana = new ObservableVariable(100f);

    public int baseCharacterHP = 350;
    public int baseCharacterStamina = 70;

    [Header("Flat Defenses")]
    public int flatIceDefense;

    [Header("Stamina Regeneration")]
    public float staminaRegenTimer = 0;
    public float staminaRegenDelay = 2;
    public float staminaTickTimer = 0;
    public float staminaTickRate = 0.1f;
    public float staminaRegenAmount = 5;

    [Header("Mana Regeneration")]
    public float manaRegenTimer = 0;
    public float manaRegenDelay = 2;
    public float manaTickTimer = 0;
    public float manaTickRate = 0.1f;
    public float manaRegenAmount = 5;

    protected virtual void Awake()
    {
        character = GetComponent<CharacterManager>();
    }
    protected virtual void Start()
    {

    }

    public int CalculateHealthBasedOnVitality(int vitality)
    {
        float health = 0;

        health = baseCharacterHP + (vitality * 15);

        return Mathf.RoundToInt(health);

    }
    protected virtual void OnEnable()
    {
        currentHealth.OnFloatChanged += CheckHP;
    }
    protected virtual void OnDisable()
    {
        currentHealth.OnFloatChanged -= CheckHP;
    }
    public int CalculateStaminaBasedOnEndurance(int endurance)
    {
        float stamina = 0;

        stamina = baseCharacterStamina + (endurance * 3);

        return Mathf.RoundToInt(stamina);
        
    }
    public void CheckHP(float oldHP, float newHP)
    {
        if (currentHealth.GetFloat() <= 0)
        {
            character.StartCoroutine(character.HandleDeathEvents());
            Debug.Log($"{character.characterName.GetString()} has died!");
        }

        if (currentHealth.GetFloat() > maxHealth.GetInt())
        {
            currentHealth.SetFloat(maxHealth.GetInt());
        }
    }

    public virtual void RegenerateStamina()
    {
        if (!character) return;

        if (character.characterMovementManager.isSprinting) return;

        if (character.isPerformingAction) return;

        staminaRegenTimer += Time.deltaTime;
        if (staminaRegenTimer >= staminaRegenDelay)
        {
            if (currentStamina.GetFloat() < maxStamina.GetInt())
            {
                staminaTickTimer += Time.deltaTime;

                if (staminaTickTimer >= staminaTickRate)
                {
                    staminaTickTimer = 0;
                    currentStamina.SetFloat(Mathf.Min(maxStamina.GetInt(), currentStamina.GetFloat() + staminaRegenAmount));
                }
            }
        }
    }

    public virtual void RegenerateMana()
    {
        if (!character) return;

        if (character.isPerformingAction) return;

        manaRegenTimer += Time.deltaTime;
        if (manaRegenTimer >= manaRegenDelay)
        {
            if (currentMana.GetFloat()  < maxMana.GetInt())
            {
                manaTickTimer += Time.deltaTime;

                if (manaTickTimer >= manaTickRate)
                {
                    manaTickTimer = 0;
                    currentMana.SetFloat(Mathf.Min(maxMana.GetInt(), currentMana.GetFloat() + manaRegenAmount));
                }
            }
        }
    }

    public virtual void ResetStaminaRegenTimer(float oldValue, float newValue)
    {
        //reset stamina regen timer only if we used stamina -> stamina went down
        //dont reset if stamina is already going up 
        if (newValue < oldValue)
        {
            staminaRegenTimer = 0;
        }

    }
    public virtual void ResetManaRegenTimer(float oldValue, float newValue)
    {
        //reset mana regen timer only if we used mana -> mana went down
        //dont reset if stamina is already going up 
        if (newValue < oldValue)
        {
            manaRegenTimer = 0;
        }

    }
}
