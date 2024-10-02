using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.AI.Navigation;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public enum ChaosBotState { Patrolling, Chasing, Engaging, Searching }
[RequireComponent(typeof(FieldOfView))]
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
    [SerializeField] List<GameObject> oneOfPlayerObjs;
    [SerializeField] bool playerIsClose;

    // Start is called before the first frame update
    void Start()
    {
        fovLight.color = unawareColor;
        currState = ChaosBotState.Patrolling;
        navMeshSurface = PlayerController.Instance.gameObject.GetComponent<NavMeshSurface>();
        obstructionMask = fov.obstructionMask;
        randomSearch.enabled = false;

        StartCoroutine(BakeNavMeshRoutine());
    }

    // Update is called once per frame
    void Update()
    {
        // Patrolling state
        if (currState == ChaosBotState.Patrolling)
        {
            spotIcon.text = "?";
            spotBar.color = alertColor;

            // Increase/decrease spot bar when inside/outside of view
            Vector3 dirToTarget = (PlayerController.Instance.currPlayerObj.transform.position - fovLight.transform.position).normalized;
            float distanceToTarget = Vector3.Distance(fovLight.transform.position, PlayerController.Instance.currPlayerObj.transform.position);
            Debug.DrawRay(fovLight.transform.position, dirToTarget, Color.cyan);

            // When player is in view or is close and can be seen...
            if (fov.canSeePlayer || (playerIsClose &&
                !Physics.Raycast(fovLight.transform.position, dirToTarget, distanceToTarget, layersToCollide)))
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
                Debug.Log("Despotting player");
            }

            // Spot player when exposed for a certain time
            if (spotValue >= unawareSpotTime)
            {
                playerSpotted = true;
                Debug.Log("Player spotted");

                if (Vector3.Distance(transform.position, fov.lastTarget.transform.position) <= engagingRange)
                {
                    currState = ChaosBotState.Engaging;
                    Debug.Log("Engaging player");
                }
                else
                {
                    currState = ChaosBotState.Chasing;
                    Debug.Log("Chasing player");
                }
            }
            // Resume patrol when player is out of view and spotting bar is 0
            else if (spotValue <= 0 && patrolScript.patrolInterrupted)
            {
                patrolScript.ResumePatrol();
                playerSpotted = false;
                anim.SetBool("isAlerted", false);
                Debug.Log("Disengaging player");
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

            // Chase player
            if (fov.canSeePlayer)
            {
                ai.SetDestination(fov.lastTarget.transform.position);
                lastKnownPlayerPos = fov.lastTarget.transform.position;
            }

            // Start a timeout timer when player is out of view
            if (!fov.canSeePlayer && !chaseTimingOut)
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
                && !fov.canSeePlayer && isCloseToOneOfPlayerObjs)
            {
                foreach (GameObject playerObj in oneOfPlayerObjs)
                {
                    if (playerObj == fov.lastTarget)
                    {
                        Debug.Log("Line hit");
                        currState = ChaosBotState.Searching;
                        ai.isStopped = true;
                        searchingValue = searchTimeoutTime;
                        spotValue = 0f;

                        chaseTimingOut = false;

                        StopCoroutine(ChaseTimeout(5f));
                        StopCoroutine(ChaseTimeout(2f));
                    }
                }
            }
            // Interrupt timeout when player is back in view
            else if (fov.canSeePlayer)
            {
                StopCoroutine(ChaseTimeout(5f));
                chaseTimingOut = false;
                Debug.Log("Chase continued");
            }

            // Switch to engaging state when entering firing range
            if (fov.canSeePlayer && Vector3.Distance(transform.position, fov.lastTarget.transform.position) < engagingRange)
            {
                currState = ChaosBotState.Engaging;
                ai.isStopped = true;

                Debug.Log("Switching to Engaging state");
            }
        }

        else if (currState == ChaosBotState.Engaging)
        {
            Debug.Log("Entered Engaging state");

            spotBar.fillAmount = 1;
            spotIcon.text = "!!";
            spotBar.color = engagingColor;
            fovLight.color = engagingColor;

            anim.SetBool("isMoving", false);

            ai.SetDestination(fov.lastTarget.transform.position);
            lastKnownPlayerPos = fov.lastTarget.transform.position;

            // Chase player when out of view
            if (!fov.canSeePlayer || Vector3.Distance(transform.position, fov.lastTarget.transform.position) > engagingRange + (engagingRange / 2))
            {
                currState = ChaosBotState.Chasing;

                Debug.Log("Switching to chasing state");
            }
            else // Otherwise update lastKnownPlayerPos
            {
                lastKnownPlayerPos = fov.lastTarget.transform.position;
                LookTowardsTarget();
            }
        }

        else if (currState == ChaosBotState.Searching)
        {
            Debug.Log("Entered Searching state");

            randomSearch.enabled = true;
            randomSearch.centrePoint.transform.position = lastKnownPlayerPos;
            ai.isStopped = false;

            fovLight.color = searchingColor;
            spotIcon.text = "??";
            spotBar.color = searchingColor;

            anim.SetBool("isMoving", false);
            anim.SetBool("isAlerted", true);

            // Play a searching animation

            Vector3 dirToTarget = (PlayerController.Instance.currPlayerObj.transform.position - fovLight.transform.position).normalized;
            float distanceToTarget = Vector3.Distance(fovLight.transform.position, PlayerController.Instance.currPlayerObj.transform.position);
            Debug.DrawRay(fovLight.transform.position, dirToTarget, Color.cyan);

            // When player is in view or is close and can be seen...
            if (fov.canSeePlayer || (playerIsClose &&
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

                Debug.Log("Spotting player");
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
                // Continue to decrease search timer
                else
                {
                    spotValue = 0f;
                    searchingValue -= despottingRate * Time.deltaTime;
                    spotBar.fillAmount = searchingValue / searchTimeoutTime;
                    spotBar.color = searchingColor;
                }

                Debug.Log("Despotting player");
            }

            // Spot player when in view for a certain time
            if (spotValue >= searchingSpotTime)
            {
                playerSpotted = true;
                randomSearch.enabled = false;
                Debug.Log("Player respotted");

                // Switch to engaging state when in range
                if (Vector3.Distance(transform.position, fov.lastTarget.transform.position) <= engagingRange)
                {
                    currState = ChaosBotState.Engaging;
                    Debug.Log("Engaging player");
                }
                else // Otherwise switch to chasing state
                {
                    currState = ChaosBotState.Chasing;
                    Debug.Log("Chasing player");
                }
            }

            if (searchingValue <= 0)
            {
                currState = ChaosBotState.Patrolling;
                spotValue = 0;
                ai.isStopped = false;
                patrolScript.ResumePatrol();
                playerSpotted = false;
                anim.SetBool("isAlerted", false);
                randomSearch.enabled = false;

                Debug.Log("Returning to patrol");
            }
        }
    }

    IEnumerator ChaseTimeout(float time)
    {
        // Variable to check whether this coroutine has already been started or not
        chaseTimingOut = true;
        Debug.Log("Chase timing out");

        yield return new WaitForSeconds(time);

        // Switch to searching state when chase has been timed out
        if (!fov.canSeePlayer)
        {
            currState = ChaosBotState.Searching;
            ai.isStopped = true;
            searchingValue = searchTimeoutTime;
            spotValue = 0f;

            Debug.Log("Chase timed out, switching to Searching state");
        }
        else
            Debug.Log("Chase continued");

        chaseTimingOut = false;
    }

    // Smoothly turns towards target
    void LookTowardsTarget()
    {
        if (!playerIsClose)
        {
            var targetRotation = Quaternion.LookRotation(fov.lastTarget.transform.position - transform.position);
            targetRotation.x = 0;
            targetRotation.z = 0;
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation,
                turnSpeed * Time.deltaTime);
        }
        else
        {
            var targetRotation = Quaternion.LookRotation(PlayerController.Instance.currPlayerObj.transform.position - transform.position);
            targetRotation.x = 0;
            targetRotation.z = 0;
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation,
                turnSpeed * Time.deltaTime);
        }
    }

    IEnumerator BakeNavMeshRoutine()
    {
        yield return new WaitForSeconds(3f);

        navMeshSurface.BuildNavMesh();
        StartCoroutine(BakeNavMeshRoutine());
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 7)
        {
            if (other.gameObject == PlayerController.Instance.currPlayerObj)
                playerIsClose = true;

            isCloseToOneOfPlayerObjs = true;

            oneOfPlayerObjs.Add(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 7)
        {
            if (other.gameObject == PlayerController.Instance.currPlayerObj)
                playerIsClose = false;

            isCloseToOneOfPlayerObjs = false;

            oneOfPlayerObjs.Remove(other.gameObject);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == 7)
        {
            if (other.gameObject == PlayerController.Instance.currPlayerObj)
                playerIsClose = true;
            else playerIsClose = false;
        }
    } 
}
