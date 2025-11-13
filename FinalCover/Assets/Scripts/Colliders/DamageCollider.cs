using System.Collections.Generic;
using UnityEngine;

public class DamageCollider : MonoBehaviour
{
    [Header("Collider")]
    [SerializeField] protected Collider damageCollider;


    [Header("Damage Types")]
    public float physicalDamage = 0; //break down into sub types (standard, slash, pierce, strike)
    public float fireDamage = 0;
    public float lightningDamage = 0;
    public float iceDamage = 0;
    public float poisonDamage = 0;
    public float decayDamage = 0;
    public float shadowDamage = 0;

    [Header("Contact Point")]
    protected Vector3 contactPoint;

    [Header("Characters Damaged")]
    protected List<CharacterManager> charactersDamaged = new List<CharacterManager>();

    protected virtual void Awake()
    {

    }
    protected virtual void Start()
    {

    }
    protected virtual void Update()
    {

    }

    protected virtual void OnTriggerEnter(Collider col)
    {
        CharacterManager damageTarget = col.GetComponentInParent<CharacterManager>();
        //Debug.Log("hit");

        if (damageTarget != null)
        {
            contactPoint = damageTarget.GetComponent<Collider>().ClosestPointOnBounds(transform.position);

            //check if we can damage this target or not based on characters "freindly fire" 


            //check if target is blocking


            //check if target is invulnerable
            if (damageTarget.characterCombatManager.isInvulnerable) return;

            DamageTarget(damageTarget);

        }
    }

    protected virtual void DamageTarget(CharacterManager damageTarget)
    {
        //dont want to deal damage again to a target if we already damaged them with this instance of damage
        //add them to a list and check the list to see if they are on the list of damageable characters already or not 
        if (charactersDamaged.Contains(damageTarget)) return;
        charactersDamaged.Add(damageTarget);

        TakeDamageEffect damageEffect = Instantiate(WorldCharacterEffectsManager.instance.takeDamageEffect);
        
        damageEffect.physicalDamage = physicalDamage;
        damageEffect.fireDamage = fireDamage;
        damageEffect.lightningDamage = lightningDamage;
        damageEffect.iceDamage = iceDamage;
        damageEffect.poisonDamage = poisonDamage;
        damageEffect.shadowDamage = shadowDamage;
        damageEffect.decayDamage = decayDamage;
        damageEffect.contactPoint = contactPoint;

        damageTarget.characterEffectsManager.ProcessInstantEffects(damageEffect);
    }
    public virtual void EnableDamageCollider()
    {
        damageCollider.enabled = true;
    }
    public virtual void DisableDamageCollider()
    {
        damageCollider.enabled = false;
        charactersDamaged.Clear(); //reset the characters list so you can damage characters on the next attack
    }
}
