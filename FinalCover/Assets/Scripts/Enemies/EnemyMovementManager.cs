using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyMovementManager : CharacterMovementManager
{
    EnemyCharacterManager enemy;

    [SerializeField] float rotationSpeed = 5f;

    [Header("Manual Motion Values")]
    public float motionMultiplier = 1f;
    public bool manualMotionActive = false;
    public bool useAnimDuration = true;
    [SerializeField] float totalManualDistance = 1f;
    [SerializeField] float manualMotionDurationNormalized = 1f;
    [SerializeField] ManualMotionDirection manualMovementDir = ManualMotionDirection.LocalForward;
    [SerializeField] AnimationCurve manualMotionCurve = AnimationCurve.Linear(0,0,1,1);
    private Vector3 manualMovementVector;
    private float manualMotionDuration = 0.01f;
    private float manualMotionTimer = 0f;
    private float manualMotionSoFar = 0f;

    protected override void Awake()
    {
        base.Awake();
        enemy = GetComponent<EnemyCharacterManager>();
    }
    protected override void Update()
    {
        base.Update();
        UpdateAnimatorMovementValues();
    }
    public void RotateWithMovement(EnemyCharacterManager enemy)
    {
        if (enemy.enemyMovementManager.isMoving.GetBool()) //if the ai character is moving do this 
        {
            enemy.transform.rotation = enemy.navMeshAgent.transform.rotation;
        }
    }
    public void RotateToFaceTarget(EnemyCharacterManager enemy)
    {
        var cm = enemy.enemyCombatManager;

        if (cm == null || cm.currentTarget == null) return;

        if (!enemy.canRotate) return;

        Vector3 dir = cm.currentTarget.transform.position - enemy.transform.position;
        dir.y = 0f;

        if (dir.sqrMagnitude < 0.0001f)
            return;

        dir.Normalize();
        Quaternion targetRot = Quaternion.LookRotation(dir);
        Quaternion finalRot = Quaternion.Slerp(
            enemy.transform.rotation,
            targetRot,
            rotationSpeed * Time.deltaTime
        );

        enemy.transform.rotation = finalRot;
    }
    private void UpdateAnimatorMovementValues()
    {
        if (enemy.navMeshAgent == null) return;

        Vector3 worldVel = enemy.navMeshAgent.desiredVelocity;

        // if not moving, set all to zero
        if (worldVel.sqrMagnitude < 0.001f)
        {
            enemy.horizontalMovement = 0;
            enemy.verticalMovement = 0;
            enemy.moveAmount = 0;
            enemy.enemyAnimationManager.UpdateAnimatorMovementParameters(0, 0, false);
            return;
        }

        // convert to local for left/right and fwd/back
        Vector3 localVel = enemy.transform.InverseTransformDirection(worldVel.normalized);

        enemy.verticalMovement = Mathf.Clamp(localVel.z, -1f, 1f);
        enemy.horizontalMovement = Mathf.Clamp(localVel.x, -1f, 1f);
        enemy.moveAmount = Mathf.Clamp01(Mathf.Abs(enemy.verticalMovement) + Mathf.Abs(enemy.horizontalMovement));

        enemy.enemyAnimationManager.UpdateAnimatorMovementParameters(
            enemy.horizontalMovement, enemy.verticalMovement, false);
    }

    public void SetManualMotionValuesRemotely(float distance, float durNorm, ManualMotionDirection moveDir, AnimationCurve curve)
    {
        totalManualDistance = distance;
        manualMotionDurationNormalized = durNorm;
        manualMovementDir = moveDir;
        manualMotionCurve = curve;
    }
    public void StartManualMotion()
    {
        if (manualMotionActive) return;

        
        //if in attack state, get the manualTimeValues from the currentAttack
        if (enemy.currentState is AttackState)
        {
            totalManualDistance = enemy.attack.currentAttack.manualMotionDistance;
            manualMotionDurationNormalized = enemy.attack.currentAttack.manualMotionDurationNormalized;
            manualMovementDir = enemy.attack.currentAttack.manualMotionDirection;
            manualMotionCurve = enemy.attack.currentAttack.manualMotionCurve;
            useAnimDuration = enemy.attack.currentAttack.durationFromClipLength;
            manualMotionDuration = enemy.attack.currentAttack.manualMovementDuration;
        }

        //if in CombatStance, then it isnt an attack, so use the values already existing
        //since they should have been updated by the dodge
        else
        {
            //all values are set by the functions that call the functions (dodge, etc)
        }

        //extra multiplier for buffs/phases/etc
        totalManualDistance *= motionMultiplier;

        //get the actual direction of movement
        manualMovementVector = GetManualMotionDirection(enemy, manualMovementDir);

        if (useAnimDuration) //calculate duration from the animation clip
        {

            //using the other values, determine the duration to space the movement out over
            Animator anim = enemy.animator;
            AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);

            //duration = find how much time is left in the current amin clip
            float clipLength = stateInfo.length / Mathf.Max(0.01f, anim.speed); ;

            manualMotionDuration = clipLength * manualMotionDurationNormalized;
        }
        else //i need to have passed the duration manually instead
        {
            //use the already passed in duration, i f i need to get it a different way do it here
        }

        //in case the duration is too short
        if (manualMotionDuration < 0.001f) manualMotionDuration = 0.001f;

        //reset stuff for the anim timer
        manualMotionTimer = 0f;
        manualMotionSoFar = 0f;

        // turn off navMesh while moving manually so it doesnt compete
        if (enemy.navMeshAgent != null)
        {
            enemy.navMeshAgent.isStopped = true;
        }

        // make sure root motoion is off
        enemy.applyRootMotion = false;

        manualMotionActive = true;
    }
    public void HandleManualMotion()
    {
        if (!manualMotionActive) return;

        manualMotionTimer += Time.deltaTime;
        float t = Mathf.Clamp01(manualMotionTimer/manualMotionDuration);

        if(manualMotionCurve != null)
        {
            t = manualMotionCurve.Evaluate(t);
        }

        //how much to move this frame based on last frame pos 
        float distanceSoFar = Mathf.Lerp(0f, totalManualDistance, t);
        float frameDistance = distanceSoFar - manualMotionSoFar;
        manualMotionSoFar = distanceSoFar;

        Vector3 moveAmountThisFrame = manualMovementVector * frameDistance;
        enemy.characterController.Move(moveAmountThisFrame);

        //end based on duration
        if (manualMotionTimer >= manualMotionDuration)
        {
            manualMotionActive = false;

            if (enemy.navMeshAgent != null)
                enemy.navMeshAgent.isStopped = false;

            enemy.applyRootMotion = true;
        }
    }
    private Vector3 GetManualMotionDirection(EnemyCharacterManager enemy, ManualMotionDirection thisDirection)
    {
        var cm = enemy.enemyCombatManager;
        Vector3 forward = enemy.transform.forward;
        Vector3 right = enemy.transform.right;
        Vector3 up = enemy.transform.up;

        switch (thisDirection)
        {
            case ManualMotionDirection.LocalForward:
                return forward;
            case ManualMotionDirection.LocalBackward:
                return -forward;
            case ManualMotionDirection.LocalRight:
                return right;
            case ManualMotionDirection.LocalLeft:
                return -right;
            case ManualMotionDirection.LocalUp:
                return up; ;
            case ManualMotionDirection.LocalDown:
                return -up;
            case ManualMotionDirection.TowardsTarget:
                if (cm.currentTarget != null)
                    return (cm.currentTarget.transform.position - enemy.transform.position).normalized;
                return forward;
            case ManualMotionDirection.AwayFromTarget:
                if (cm.currentTarget != null)
                    return (enemy.transform.position - cm.currentTarget.transform.position).normalized;
                return -forward;
            default:
                return Vector3.zero;
        }
    }
}
