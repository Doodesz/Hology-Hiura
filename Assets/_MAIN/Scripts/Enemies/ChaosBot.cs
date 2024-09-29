using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum ChaosBotState { Patrolling, Spotting, Chasing, Engaging, Searching }
[RequireComponent(typeof(FieldOfView))]
public class ChaosBot : MonoBehaviour
{
    [Header("References")]
    [SerializeField] FieldOfView fov;
    [SerializeField] Light fovLight;
    [SerializeField] Patrol patrolScript;
    [SerializeField] Animator anim;

    [Header("Variables")]
    [SerializeField] float spotValue;
    [SerializeField] float unawareSpotTime = 2f;
    [SerializeField] float searchingSpotTime = 1f;
    [SerializeField] float spottingRate = 1f;
    [SerializeField] float despottingRate = 0.5f;
    [SerializeField] float unawareSpottingTimeout = 3f;
    [SerializeField] float engagingRange = 5f;
    
    public float speed = 10f;
    public float turnSpeed = 5f;

    [Header("Lights")]
    [SerializeField] Color unawareColor;
    [SerializeField] Color alertColor;
    [SerializeField] Color engagingColor;


    [Header("Debugging")]
    public bool playerSpotted;
    [SerializeField] ChaosBotState currState;

    // Start is called before the first frame update
    void Start()
    {
        fovLight.color = unawareColor;
        currState = ChaosBotState.Patrolling;
    }

    // Update is called once per frame
    void Update()
    {
        // Patrolling state
        if (currState == ChaosBotState.Patrolling)
        {
            // Increase/decrease spot bar when inside/outside of view
            if (fov.canSeePlayer)
            {
                patrolScript.InterruptPatrol();
                spotValue += spottingRate * Time.deltaTime;
                fovLight.color = alertColor;
                anim.SetBool("isAlerted", true);

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
                fovLight.color = unawareColor;
                Debug.Log("Despotting player");
            }

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
            else if (spotValue <= 0 && patrolScript.patrolInterrupted)
            {
                patrolScript.ResumePatrol();
                playerSpotted = false;
                anim.SetBool("isAlerted", false);
                Debug.Log("Disengaging player");
            }

        }

    }
}
