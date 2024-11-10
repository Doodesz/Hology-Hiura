using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using Unity.AI.Navigation;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;

public enum ChaosBotState { Patrolling, Chasing, Engaging, Searching }
[RequireComponent(typeof(FieldOfView), typeof(ObjectSoundManager))]
public class ChaosBot : MonoBehaviour
{
    [Header("References")]
    [SerializeField] FieldOfView fov;
    [SerializeField] Light fovLight;
    [SerializeField] Patrol patrolScript;
    [SerializeField] Animator anim;
    [SerializeField] Image spotBar;
    [SerializeField] TextMeshProUGUI spotIcon;
    [SerializeField] NavMeshAgent ai;
    [SerializeField] RandomMovement randomSearch;
    [SerializeField] GameObject shootLaserObj;
    [SerializeField] GameObject laserSpawnPos;
    [SerializeField] ObjectSoundManager soundManager;

    [Header("Variables")]
    [SerializeField] float spotValue;
    [SerializeField] float searchingValue;
    [SerializeField] float unawareSpotTime = 2f;
    [SerializeField] float searchingSpotTime = 1f;
    [SerializeField] float searchTimeoutTime = 5f;
    [SerializeField] float spottingRate = 1f;
    [SerializeField] float despottingRate = 0.5f;
    [SerializeField] float unawareSpottingTimeout = 3f;
    [SerializeField] float engagingRange = 5f;
    [SerializeField] LayerMask layersToCollide;

    public float speed = 10f;
    public float turnSpeed = 5f;

    [Header("Lights")]
    [SerializeField] Color unawareColor;
    [SerializeField] Color alertColor;
    [SerializeField] Color engagingColor;
    [SerializeField] Color searchingColor;

    [Header("Debugging")]
    public bool playerSpotted;
    [SerializeField] ChaosBotState currState;
    [SerializeField] bool chaseTimingOut;
    [SerializeField] Vector3 lastKnownPlayerPos;
    [SerializeField] NavMeshSurface navMeshSurface;
    [SerializeField] LayerMask obstructionMask;
    [SerializeField] bool isCloseToOneOfPlayerObjs;
    [SerializeField] List<GameObject> oneOfClosePlayerObjs;
    [SerializeField] bool electronicIsClose;
    [SerializeField] bool isChargingShot;
    [SerializeField] bool isStuck;
    [SerializeField] float stuckTimeoutValue;
    [SerializeField] float stuckTimeout = 3.5f;
    [SerializeField] Vector3 lastPos;

    // Start is called before the first frame update
    void Start()
    {
        fovLight.color = unawareColor;
        currState = ChaosBotState.Patrolling;
        navMeshSurface = PlayerController.Instance.gameObject.GetComponent<NavMeshSurface>();
        obstructionMask = fov.obstructionMask;
        randomSearch.enabled = false;
        lastPos = Vector3.zero;

        StartCoroutine(BakeNavMeshRoutine());
    }

    // Update is called once per frame
    void Update()
    {
        // Determines if stuck or not
        if (lastPos == transform.position)
            stuckTimeoutValue += Time.deltaTime;
        else
            stuckTimeoutValue = 0f;

        if (stuckTimeoutValue > stuckTimeout)
        {
            isStuck = true;
        }


        // Fix for when player enters trigger box but not in chaosbot's view
        if (fov.lastTarget == null)
        { 
            float range = 100f;

            foreach (GameObject obj in oneOfClosePlayerObjs)
            {
                if (Vector3.Distance(obj.transform.position, gameObject.transform.position) < range)
                {
                    fov.lastTarget = obj;
                    range = Vector3.Distance(obj.transform.position, gameObject.transform.position);
                }
            }
        }

        // Patrolling state
        if (currState == ChaosBotState.Patrolling)
        {
            spotIcon.text = "?";
            spotBar.color = alertColor;

            // Increase/decrease spot bar when inside/outside of view
            Vector3 dirToTarget = Vector3.zero;
            float distanceToTarget = 0f;
            
            if (fov.lastTarget != null)
            {
                dirToTarget = (fov.lastTarget.transform.position - fovLight.transform.position).normalized;
                distanceToTarget = Vector3.Distance(fovLight.transform.position, fov.lastTarget.transform.position);
                
                //Debug.DrawRay(fovLight.transform.position, dirToTarget, Color.cyan);
            }

            // When player is in view or is close and can be seen...
            if ((fov.canSeePlayerElectronic || (electronicIsClose &&
                !Physics.Raycast(fovLight.transform.position, dirToTarget, distanceToTarget, layersToCollide)))
                && fov.lastTarget.GetComponent<ControllableElectronic>().isOnline)
            {
                patrolScript.InterruptPatrol();
                spotValue += spottingRate * Time.deltaTime;
                fovLight.color = alertColor;
                anim.SetBool("isAlerted", true);

                LookTowardsTarget();
            }
            else
            {
                if (spotValue > 0)
                    spotValue -= despottingRate * Time.deltaTime;
                fovLight.color = unawareColor;
                //Debug.Log("Despotting player");
            }

            // Spot player when exposed for a certain time and can be chased
            if (spotValue >= unawareSpotTime)
            {
                ai.CalculatePath(lastKnownPlayerPos, ai.path);
                if (ai.pathStatus != NavMeshPathStatus.PathInvalid 
                    || Vector3.Distance(transform.position, fov.lastTarget.transform.position) <= engagingRange)
                {
                    playerSpotted = true;
                    //Debug.Log("Player spotted");


                    if (Vector3.Distance(transform.position, fov.lastTarget.transform.position) <= engagingRange)
                    {
                        currState = ChaosBotState.Engaging;
                        //Debug.Log("Engaging player");
                    }
                    else 
                    {
                        currState = ChaosBotState.Chasing;
                        //Debug.Log("Chasing player");
                    }
                }
                else
                    spotValue = unawareSpotTime;
            }
            // Resume patrol when player is out of view and spotting bar is 0
            else if (spotValue <= 0 && patrolScript.patrolInterrupted)
            {
                patrolScript.ResumePatrol();
                playerSpotted = false;
                anim.SetBool("isAlerted", false);
                //Debug.Log("Disengaging player");
            }

            // Update radial spotting bar
            if (spotValue > 0)
            {
                float fillValue = spotValue / unawareSpotTime;
                spotBar.fillAmount = fillValue;
                spotIcon.enabled = true;
            }
            else
            {
                spotBar.fillAmount = 0;
                spotIcon.enabled = false;
            }

            // Limits max value
            if (spotValue > unawareSpotTime)
                spotValue = unawareSpotTime;
        }

        // Chasing state
        else if (currState == ChaosBotState.Chasing)
        {
            spotBar.fillAmount = 1;
            spotIcon.text = "!";
            spotBar.color = engagingColor;
            fovLight.color = engagingColor;

            anim.SetBool("isMoving", true);

            ai.isStopped = false;

            ai.CalculatePath(lastKnownPlayerPos, ai.path);

            // Chase player
            if (fov.canSeePlayerElectronic && ai.pathStatus != NavMeshPathStatus.PathInvalid)
            {
                ai.SetDestination(fov.lastTarget.transform.position);
                lastKnownPlayerPos = fov.lastTarget.transform.position;
            }

            // Return to patrol when target is already offline
            if (!fov.lastTarget.GetComponent<ControllableElectronic>().isOnline)
            {
                currState = ChaosBotState.Patrolling;
                spotValue = 0;
                ai.isStopped = false;
                patrolScript.ResumePatrol();
                playerSpotted = false;
                anim.SetBool("isAlerted", false);
                randomSearch.enabled = false;
                StopAllCoroutines();
                StartCoroutine(BakeNavMeshRoutine()); 
                isChargingShot = false;

                if (oneOfClosePlayerObjs.Contains(fov.lastTarget.gameObject))
                    oneOfClosePlayerObjs.Remove(fov.lastTarget.gameObject);

                //Debug.Log("Returning to patrol");
            }

            // Start a timeout timer when player is out of view
            if (!fov.canSeePlayerElectronic && !chaseTimingOut)
            {
                StartCoroutine(ChaseTimeout(5f));
                chaseTimingOut = true;
            }
            else if (ai.remainingDistance <= 0)
            {
                currState = ChaosBotState.Searching;
                ai.isStopped = true;
                anim.SetBool("isMoving", false);
                searchingValue = searchTimeoutTime;
                spotValue = 0f;

                chaseTimingOut = false;

                StopCoroutine(ChaseTimeout(5f));
                StopCoroutine(ChaseTimeout(2f));
            }
            // Immediately enter searching state when near lastKnownPlayerPos and player is not in view
            else if (Vector3.Distance(transform.position, lastKnownPlayerPos) < 1f
                && !fov.canSeePlayerElectronic)
            {
                //Debug.Log("Line hit");
                currState = ChaosBotState.Searching;
                ai.isStopped = true;
                searchingValue = searchTimeoutTime;
                spotValue = 0f;

                chaseTimingOut = false;

                StopAllCoroutines();
                StartCoroutine(BakeNavMeshRoutine());
            }
            // Interrupt timeout when player is back in view
            else if (fov.canSeePlayerElectronic)
            {
                StopAllCoroutines();
                StartCoroutine(BakeNavMeshRoutine()); 
                chaseTimingOut = false;
                //Debug.Log("Chase continued");
            }

            // Switch to engaging state when entering firing range
            if (fov.canSeePlayerElectronic && Vector3.Distance(transform.position, fov.lastTarget.transform.position) < engagingRange)
            {
                currState = ChaosBotState.Engaging;
                ai.isStopped = true;

                //Debug.Log("Switching to Engaging state");
            }
        }

        // Engaging state
        else if (currState == ChaosBotState.Engaging)
        {
            //Debug.Log("Entered Engaging state");

            spotBar.fillAmount = 1;
            spotIcon.text = "!!";
            spotBar.color = engagingColor;
            fovLight.color = engagingColor;

            anim.SetBool("isMoving", false);

            ai.SetDestination(fov.lastTarget.transform.position);
            lastKnownPlayerPos = fov.lastTarget.transform.position;

            // Increase/decrease spot bar when inside/outside of view
            Vector3 dirToTarget = Vector3.zero;
            float distanceToTarget = 0f;

            if (fov.lastTarget != null)
            {
                dirToTarget = (fov.lastTarget.transform.position - fovLight.transform.position).normalized;
                distanceToTarget = Vector3.Distance(fovLight.transform.position, fov.lastTarget.transform.position);

                //Debug.DrawRay(fovLight.transform.position, dirToTarget, Color.cyan);
            }

            // Return to patrol when target is already offline
            if (!fov.lastTarget.GetComponent<ControllableElectronic>().isOnline)
            {
                currState = ChaosBotState.Patrolling;
                spotValue = 0;
                ai.isStopped = false;
                patrolScript.ResumePatrol();
                playerSpotted = false;
                anim.SetBool("isAlerted", false);
                randomSearch.enabled = false;
                StopAllCoroutines();
                StartCoroutine(BakeNavMeshRoutine()); 
                isChargingShot = false;

                if (oneOfClosePlayerObjs.Contains(fov.lastTarget.gameObject))
                    oneOfClosePlayerObjs.Remove(fov.lastTarget.gameObject);

                //Debug.Log("Returning to patrol");
            }

            //ai.CalculatePath(lastKnownPlayerPos, ai.path);

            // When player is not in view or is far...
            if (((!fov.canSeePlayerElectronic && !(electronicIsClose &&
                !Physics.Raycast(fovLight.transform.position, dirToTarget, distanceToTarget, layersToCollide))) 
                || Vector3.Distance(transform.position, fov.lastTarget.transform.position) > engagingRange + (engagingRange / 2))
                && fov.lastTarget.GetComponent<ControllableElectronic>().isOnline && ai.pathStatus == NavMeshPathStatus.PathComplete)
            {
                currState = ChaosBotState.Chasing;

                //Debug.Log("Switching to chasing state");

                StopAllCoroutines();
                StartCoroutine(BakeNavMeshRoutine()); 
                isChargingShot = false;
            }
            else // Otherwise update lastKnownPlayerPos
            {
                lastKnownPlayerPos = fov.lastTarget.transform.position;
                LookTowardsTarget();
                if (!isChargingShot)
                    StartCoroutine(AttemptToTerminate());
            }
        }

        // Searching state
        else if (currState == ChaosBotState.Searching)
        {
            //Debug.Log("Entered Searching state");
            
            // Resets current path upon first entering state
            if (randomSearch.enabled == false)
                ai.ResetPath();

            randomSearch.enabled = true;
            randomSearch.centrePoint.transform.position = lastKnownPlayerPos;
            ai.isStopped = false;

            fovLight.color = searchingColor;
            spotIcon.text = "??";
            spotBar.color = searchingColor;

            anim.SetBool("isMoving", false);
            anim.SetBool("isAlerted", true);

            // Return to patrol when target is already offline
            if (!fov.lastTarget.GetComponent<ControllableElectronic>().isOnline)
            {
                currState = ChaosBotState.Patrolling;
                spotValue = 0;
                ai.isStopped = false;
                patrolScript.ResumePatrol();
                playerSpotted = false;
                anim.SetBool("isAlerted", false);
                randomSearch.enabled = false;
                StopAllCoroutines();
                StartCoroutine(BakeNavMeshRoutine());
                isChargingShot = false;

                if (oneOfClosePlayerObjs.Contains(fov.lastTarget.gameObject))
                    oneOfClosePlayerObjs.Remove(fov.lastTarget.gameObject);

                //Debug.Log("Returning to patrol");
            }

            // Increase/decrease spot bar when inside/outside of view
            Vector3 dirToTarget = Vector3.zero;
            float distanceToTarget = 0f;

            if (fov.lastTarget != null)
            {
                dirToTarget = (fov.lastTarget.transform.position - fovLight.transform.position).normalized;
                distanceToTarget = Vector3.Distance(fovLight.transform.position, fov.lastTarget.transform.position);
                lastKnownPlayerPos = fov.lastTarget.transform.position;
                
                Debug.DrawRay(fovLight.transform.position, dirToTarget, Color.cyan);
            }
            

            // When player is in view or is close and can be seen...
            if (fov.canSeePlayerElectronic || (electronicIsClose &&
                !Physics.Raycast(fovLight.transform.position, dirToTarget, distanceToTarget, layersToCollide)))
            {
                // Reset search timer and continue to spot player
                spotValue += spottingRate * Time.deltaTime;
                searchingValue = searchTimeoutTime;

                spotBar.fillAmount = spotValue / searchingSpotTime;

                fovLight.color = engagingColor;
                spotIcon.text = "!?";
                spotBar.color = engagingColor;

                LookTowardsTarget();

                ai.isStopped = true;

                anim.SetBool("isMoving", false);

                {   // Smoothly turns towards target
                    var targetRotation = Quaternion.LookRotation(fov.lastTarget.transform.position - transform.position);
                    targetRotation.x = 0;
                    targetRotation.z = 0;
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation,
                        turnSpeed * Time.deltaTime);
                }

                //Debug.Log("Spotting player");
            }
            else // Otherwise...
            {
                ai.isStopped = false;

                anim.SetBool("isMoving", true);

                // If previously exposed when searching, decrease that spot timer first, then...
                if (spotValue > 0)
                {
                    searchingValue = searchTimeoutTime;
                    spotValue -= despottingRate * Time.deltaTime;
                    spotBar.fillAmount = spotValue / searchingSpotTime;
                    spotBar.color = engagingColor;
                }
                // Else, continue to decrease search timer
                else
                {
                    spotValue = 0f;
                    searchingValue -= despottingRate * Time.deltaTime;
                    spotBar.fillAmount = searchingValue / searchTimeoutTime;
                    spotBar.color = searchingColor;
                }

                //Debug.Log("Despotting player");
            }

            // Spot player when in view for a certain time and can be chased
            ai.CalculatePath(lastKnownPlayerPos, ai.path);
            if (spotValue >= searchingSpotTime && ai.pathStatus == NavMeshPathStatus.PathComplete)
            {
                playerSpotted = true;
                randomSearch.enabled = false;
                //Debug.Log("Player respotted");


                // Switch to engaging state when in range
                if (Vector3.Distance(transform.position, fov.lastTarget.transform.position) <= engagingRange)
                {
                    currState = ChaosBotState.Engaging;
                    //Debug.Log("Engaging player");
                }
                // Otherwise switch to chasing state
                else
                {
                    currState = ChaosBotState.Chasing;
                    //Debug.Log("Chasing player");
                }
            }
            // Limits spotValue if can't be chased
            else if (spotValue >= searchingSpotTime)
                spotValue = searchingSpotTime;

            if (searchingValue <= 0)
            {
                currState = ChaosBotState.Patrolling;
                spotValue = 0;
                ai.isStopped = false;
                patrolScript.ResumePatrol();
                playerSpotted = false;
                anim.SetBool("isAlerted", false);
                randomSearch.enabled = false;

                //Debug.Log("Returning to patrol");
            }

            // Limits aearch value
            if (searchingValue > searchTimeoutTime)
                searchingValue = searchTimeoutTime;
        }

        // Sound
        if (!soundManager.moveSfx.isPlaying && anim.GetBool("isMoving"))
        {
            soundManager.PlayMove();
        }
        else if (soundManager.moveSfx.isPlaying && !anim.GetBool("isMoving"))
        {
            soundManager.PauseMove();
        }

        // Anim
        if (currState != ChaosBotState.Patrolling && (ai.remainingDistance == 0f || ai.isStopped))
            anim.SetBool("isMoving", false);
    }

    IEnumerator ChaseTimeout(float time)
    {
        // Variable to check whether this coroutine has already been started or not
        if (chaseTimingOut) 
            yield break;
        chaseTimingOut = true;
        //Debug.Log("Chase timing out");

        yield return new WaitForSeconds(time);

        // Switch to searching state when chase has been timed out
        if (!fov.canSeePlayerElectronic)
        {
            currState = ChaosBotState.Searching;
            ai.isStopped = true;
            searchingValue = searchTimeoutTime;
            spotValue = 0f;

            //Debug.Log("Chase timed out, switching to Searching state");
        }
        //else
            //Debug.Log("Chase continued");

        chaseTimingOut = false;
    }

    // Smoothly turns towards target
    void LookTowardsTarget()
    {
        var targetRotation = Quaternion.LookRotation(fov.lastTarget.transform.position - transform.position);
        targetRotation.x = 0;
        targetRotation.z = 0;
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation,
            turnSpeed * Time.deltaTime);

        var laserSpawnTargetRot = Quaternion.LookRotation(fov.lastTarget.transform.position - laserSpawnPos.transform.position);
        laserSpawnPos.transform.rotation = laserSpawnTargetRot;
    }

    IEnumerator BakeNavMeshRoutine()
    {
        yield return new WaitForSeconds(3f);

        if (isStuck)
        {
            navMeshSurface.BuildNavMesh();
            stuckTimeoutValue = 0f;
            isStuck = false;
        }
    
        StartCoroutine(BakeNavMeshRoutine());
    }

    IEnumerator AttemptToTerminate()
    {
        if (isChargingShot)
            yield break;
        else if (currState != ChaosBotState.Engaging)
        {
            isChargingShot = false;
            yield break;
        }
        isChargingShot = true;
        anim.SetBool("isAlerted", true);
        yield return new WaitForSeconds(2f);
        // shoot

        if (isChargingShot)
            Shoot();
    
        isChargingShot = false;

        StartCoroutine(AttemptToTerminate());
    }

    void Shoot()
    {
        //Debug.Log("laser shot");
        Instantiate(shootLaserObj, laserSpawnPos.transform.position, laserSpawnPos.transform.rotation);
    }

    private void OnTriggerStay(Collider other)
    {
        if ((other.gameObject.layer == 7 || other.gameObject.layer == 17) && other.TryGetComponent<ControllableElectronic>(out ControllableElectronic script)
            && other.GetComponent<ControllableElectronic>().isOnline)
        { 
            electronicIsClose = true;

            if (!oneOfClosePlayerObjs.Contains(other.gameObject))
                oneOfClosePlayerObjs.Add(other.gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if ((other.gameObject.layer == 7 || other.gameObject.layer == 17) && other.TryGetComponent<ControllableElectronic>(out ControllableElectronic script) 
            && other.GetComponent<ControllableElectronic>().isOnline)
        {
            electronicIsClose = true;
            oneOfClosePlayerObjs.Add(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if ((other.gameObject.layer == 7 || other.gameObject.layer == 17))
        {
            if (oneOfClosePlayerObjs.Contains(other.gameObject))
                oneOfClosePlayerObjs.Remove(other.gameObject);
        }
        if (oneOfClosePlayerObjs.Count == 0)
            electronicIsClose = false;
    }
}
