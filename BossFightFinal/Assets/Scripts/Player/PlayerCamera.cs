using System.Collections.Generic;
using UnityEngine;
using Unity.Cinemachine;
using System.Collections;


public class PlayerCamera : MonoBehaviour
{
    public static PlayerCamera instance;
    //public Camera cam; //if NOT using cinemachine
    public CinemachineCamera vCam; //if using cinemachine
    public PlayerManager player;

    [Header("Camera Values")]
    [SerializeField] float leftAndRightLookAngle, upAndDownLookAngle;
    
    [Header("Lock On")]
    [SerializeField] float lockOnRadius = 20;
    [SerializeField] float minViewableAngle = -50;
    [SerializeField] float maxViewableAngle = 50;
    [SerializeField] float lockOnTargetFollowSpeed = 0.2f;
    [SerializeField] float unlockedCameraHeight = 1.5f;
    [SerializeField] float lockedOnCameraHeight = 2.25f;
    [SerializeField] float setCameraHeightSpeed = 0.05f;
    private Coroutine cameraLockOnHeightCoroutine;
    private List<CharacterManager> availableTargets = new List<CharacterManager>();
    public CharacterManager nearestLockOnTarget;
    public CharacterManager leftLockOnTarget;
    public CharacterManager rightLockOnTarget;

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
        vCam = GetComponent<CinemachineCamera>();
        //DontDestroyOnLoad(gameObject);
        //cameraZPos = cam.transform.localPosition.z; //removed for cinemachine
    }
    public void SetPlayerAsFollowTarget()
    {
        vCam.Target.TrackingTarget = player.gameObject.GetComponentInChildren<CameraFollowTarget>().gameObject.transform;
        vCam.Target.LookAtTarget = player.gameObject.GetComponentInChildren<CameraFollowTarget>().gameObject.transform;
    }


    //Lock On logic
    public void HandleLocatingLockOnTargets()
    {
        availableTargets.Clear();

        float shortestDistance = Mathf.Infinity; //used to find the target closest to us 
        float shortestDistanceOfRightTarget = Mathf.Infinity; //find target on shortest distance on one axis to the right 
        float shortestDistanceOfLeftTarget = -Mathf.Infinity; //find target on shortest distance on one axis to the left (-)

        //get all colliders around player in a radius
        Collider[] colliders = Physics.OverlapSphere(player.transform.position, lockOnRadius, WorldUtilityManager.instance.GetCharacterLayers());

        //check all targets in the area and add available lock on targets to list 
        for (int i = 0; i < colliders.Length; i++)
        {
            CharacterManager lockOnTarget = colliders[i].GetComponent<CharacterManager>();
            if (lockOnTarget != null)
            {
                //check if they are within our field of view
                Vector3 lockOnTargetDirection = lockOnTarget.transform.position - player.transform.position;
                float distanceFromTarget = Vector3.Distance(player.transform.position, lockOnTarget.transform.position);
                float viewableAngle = Vector3.Angle(lockOnTargetDirection, vCam.transform.forward);

                //skip over lock on to targets that are dead, check next
                if (lockOnTarget.isDead) continue;

                //dont let us lock onto ourself, check next
                if (lockOnTarget.transform.root == player.transform.root) continue;

                //last, if the target is outside fov or blocked by enviro, check next target
                if (viewableAngle > minViewableAngle && viewableAngle < maxViewableAngle)
                {
                    RaycastHit hit;

                    //only check for environemnt layer only 
                    if (Physics.Linecast(player.playerCombatManager.lockOnTransform.position,
                        lockOnTarget.characterCombatManager.lockOnTransform.position, out hit, WorldUtilityManager.instance.GetEnviroLayers()))
                    {
                        //if true, we hit something and cannot lock on, try next target 
                        continue;
                    }
                    else
                    {
                        //add to the list of potential targets 
                        availableTargets.Add(lockOnTarget);
                    }
                }
            }
        }

        //sort through all potential targets to see which is the closest, that we lock on first
        for (int j = 0; j < availableTargets.Count; j++)
        {
            if (availableTargets[j] != null)
            {
                float distanceFromTarget = Vector3.Distance(player.transform.position, availableTargets[j].transform.position);
                Vector3 lockOnTargetDirection = availableTargets[j].transform.position - player.transform.position;

                if (distanceFromTarget < shortestDistance)
                {
                    shortestDistance = distanceFromTarget;
                    nearestLockOnTarget = availableTargets[j];
                }

                //if we are already locked on, search and set the nearest left/right targets
                if (player.playerCombatManager.isLockedOn)
                {
                    Vector3 relativeEnemyPosition = player.transform.InverseTransformPoint(availableTargets[j].transform.position);
                    float distanceFromLeftTarget = relativeEnemyPosition.x;
                    float distanceFromRightTarget = relativeEnemyPosition.x;

                    if (availableTargets[j] == player.playerCombatManager.currentTarget) continue;

                    //check left side for targets, make the closest one the left target
                    if (relativeEnemyPosition.x <= 0.00 && distanceFromLeftTarget > shortestDistanceOfLeftTarget)
                    {
                        shortestDistanceOfLeftTarget = distanceFromLeftTarget;
                        leftLockOnTarget = availableTargets[j];
                    }
                    //check targets to the right, make the closest one the right lock on target
                    else if (relativeEnemyPosition.x >= 0.00 && distanceFromRightTarget < shortestDistanceOfRightTarget)
                    {
                        shortestDistanceOfRightTarget = distanceFromRightTarget;
                        rightLockOnTarget = availableTargets[j];
                    }
                }
            }
            else
            {
                ClearLockOnTargets();
                player.playerCombatManager.isLockedOn = false;
            }
        }
    }
    public void ClearLockOnTargets()
    {
        nearestLockOnTarget = null;
        leftLockOnTarget = null;
        rightLockOnTarget = null;
        SetPlayerAsFollowTarget();
        availableTargets.Clear();
    }
    public IEnumerator WaitThenFindNewTarget()
    {
        while (player.isPerformingAction)
        {
            yield return null;
        }
        ClearLockOnTargets();
        HandleLocatingLockOnTargets();

        if (nearestLockOnTarget != null)
        {
            player.playerCombatManager.SetTarget(nearestLockOnTarget);
            player.playerCombatManager.isLockedOn = true;
        }
        yield return null;
    }
    public void SetNewTargetAsLookAtTarget(CharacterManager newTarget)
    {
        vCam.Target.LookAtTarget = newTarget.gameObject.transform;

        Debug.Log(vCam.Target.LookAtTarget);
    }
}
