using UnityEngine;

public class CharacterCombatManager : MonoBehaviour
{
    CharacterManager character;

    [Header("Attack Type")]
    public AttackType currentAttackType;

    [Header("Attack Target")]
    public CharacterManager currentTarget;

    [Header("Last Attack Animation Performed")]
    public string lastAttackAnimation;

    [Header("Lock On Transform")]
    public Transform lockOnTransform;

    [Header("Attack Flags")]
    public bool canPerformRollingAttack = false;
    public bool canPerformBackStepAttack = false;
    public bool isInvulnerable = false;
    public ObservableVariable isChargingAttack = new (false);
    public bool isLockedOn = false;
    public float chargeTimer = 0;

    protected virtual void Awake()
    {
        character = GetComponent<CharacterManager>();

        if (lockOnTransform == null)
        {
            lockOnTransform = GetComponentInChildren<LockOnTarget>().GetComponent<Transform>();
        }
    }
    protected virtual void Start()
    {

    }
    protected virtual void Update()
    {

    }
    protected virtual void OnEnable()
    {
        isChargingAttack.OnBoolChanged += OnIsChargingAttackChanged;
    }
    protected virtual void OnDisable()
    {
        isChargingAttack.OnBoolChanged -= OnIsChargingAttackChanged;

    }
    public virtual void SetTarget(CharacterManager newTarget)
    {
        if (newTarget != null)
        {
            currentTarget = newTarget;
        }
        else
        {
            currentTarget = null;
        }
    }
    public void OnIsChargingAttackChanged(bool old, bool isChargingAttack)
    {
        character.animator.SetBool("IsChargingAttack", isChargingAttack);
    }
    public virtual void EnableCanDoCombo()
    {

    }
    public virtual void DisableCanDoCombo()
    {

    }
    public void EnableIFrames()
    {
        isInvulnerable = true;
    }
    public void DisableIFrames()
    {
        isInvulnerable = false;
    }
    public void EnableCanDoRollingAttack()
    {
        canPerformRollingAttack = true;
    }
    public void DisableCanDoRollingAttack()
    {
        canPerformRollingAttack = false;
    }
    public void EnableCanDoBackStepAttack()
    {
        canPerformBackStepAttack = true;
    }
    public void DisableCanDoBackStepAttack()
    {
        canPerformBackStepAttack = false;
    }

}
