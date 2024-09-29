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
    [SerializeField] LayerMask chaseIgnoreCollider;
    
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

    // Start is called before the first frame update
    void Start()
    {
        fovLight.color = unawareColor;
        currState = ChaosBotState.Patrolling;
        navMeshSurface = PlayerController.Instance.gameObject.GetComponent<NavMeshSurface>();
        StartCoroutine(BakeNavMeshRoutine());
    }

    // Update is called once per frame
    void Update()
    {
        // Patrolling state
        if (currState == ChaosBotState.Patrolling)
        {
            spotIcon.text = "?";
            spotBar.color = Color.yellow;

            // Increase/decrease spot bar when inside/outside of view
            if (fov.canSeePlayer)
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

                if (Vector3.Distance(transform.position, fov.target.transform.position) <= engagingRange)
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
            spotBar.color = Color.red;

            ai.isStopped = false;

            // Go to player
            if (fov.target != null)
            {
                ai.SetDestination(fov.target.transform.position);
                lastKnownPlayerPos = fov.target.transform.position;
            }
            else
                ai.SetDestination(lastKnownPlayerPos);

            anim.SetBool("isMoving", true);

            // Start a timeout timer when player is out of view
            if (!fov.canSeePlayer && !chaseTimingOut)
            {
                StartCoroutine(ChaseTimeout());
            }
            else
            {
                StopCoroutine(ChaseTimeout());
                chaseTimingOut = false;
                Debug.Log("Chase continued");
            }

            // Switch to engaging state when entering firing range
            if (Vector3.Distance(transform.position, fov.target.transform.position) < engagingRange)
            {
                currState = ChaosBotState.Engaging;
                ai.isStopped = true;

                Debug.Log("Switching to engaging state");
            }
        }

        else if (currState == ChaosBotState.Engaging)
        {
            Debug.Log("Entered Engaging state");
            
            spotBar.fillAmount = 1;
            spotIcon.text = "!!";
            spotBar.color = Color.red;
            fovLight.color = Color.red;

            anim.SetBool("isMoving", false);

            LookTowardsTarget();

            // Start a timeout timer when player is out of view
            if (!fov.canSeePlayer && !chaseTimingOut)
            {
                StartCoroutine(ChaseTimeout());
            }
            else
            {
                StopCoroutine(ChaseTimeout());
                chaseTimingOut = false;
                Debug.Log("Chase continued");
            }

            // Switch to chasing state when entering firing range
            if (Vector3.Distance(transform.position, fov.target.transform.position) > engagingRange)
            {
                currState = ChaosBotState.Chasing;

                Debug.Log("Switching to chasing state");
            }
        }

        else if (currState == ChaosBotState.Searching)
        {
            Debug.Log("Entered Searching state");

            fovLight.color = alertColor;
            spotIcon.text = "??";
            spotBar.color = Color.blue;

            searchingValue -= despottingRate * Time.deltaTime;

            spotBar.fillAmount = searchingValue / searchTimeoutTime;

            anim.SetBool("isMoving", false);
            anim.SetBool("isAlerted", true);

            // Play a searching animation

            // Increase/decrease spot bar when inside/outside of view
            if (fov.canSeePlayer)
            {
                spotValue += spottingRate * Time.deltaTime;
                searchingValue = searchTimeoutTime;

                spotBar.fillAmount = spotValue / searchingSpotTime;

                fovLight.color = alertColor;
                spotIcon.text = "!?";
                spotBar.color = Color.red;

                // Smoothly turns towards target
                var targetRotation = Quaternion.LookRotation(fov.target.transform.position - transform.position);
                targetRotation.x = 0;
                targetRotation.z = 0;
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation,
                    turnSpeed * Time.deltaTime);

                Debug.Log("Spotting player");
            }
            else
            {
                if (spotValue > 0)
                    spotValue -= despottingRate * Time.deltaTime;
                else
                    searchingValue -= despottingRate * Time.deltaTime;
               

                fovLight.color = Color.blue;
                Debug.Log("Despotting player");
            }

            // Spot player when exposed for a certain time
            if (spotValue >= searchingSpotTime)
            {
                playerSpotted = true;
                Debug.Log("Player respotted");

                if (Vector3.Distance(transform.position, fov.target.transform.position) <= engagingRange)
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

            if (searchingValue <= 0)
            {
                currState = ChaosBotState.Patrolling;
                spotValue = 0;
                ai.isStopped = false;
                patrolScript.ResumePatrol();
                playerSpotted = false;
                anim.SetBool("isAlerted", false);

                Debug.Log("Returning to patrol");
            }
        }

    }

    IEnumerator ChaseTimeout()
    {
        // Variable to check whether this coroutine has already been started or not
        chaseTimingOut = true;
        Debug.Log("Chase timing out");

        yield return new WaitForSeconds(5f);

        // Switch to searching state when chase has been timed out
        if (!fov.canSeePlayer)
        {
            currState = ChaosBotState.Searching;
            ai.isStopped = true;
            searchingValue = 5f;
            spotValue = 0f;

            Debug.Log("Chase timed out, switching to Searching state");
        }
        else
            Debug.Log("Chase continued");

        chaseTimingOut = false;
    }

    void LookTowardsTarget()
    {
        // Smoothly turns towards target
        var targetRotation = Quaternion.LookRotation(fov.target.transform.position - transform.position);
        targetRotation.x = 0;
        targetRotation.z = 0;
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation,
            turnSpeed * Time.deltaTime);
    }

    IEnumerator BakeNavMeshRoutine()
    {
        yield return new WaitForSeconds(3f);

        navMeshSurface.BuildNavMesh();
        StartCoroutine(BakeNavMeshRoutine());
    }
}
