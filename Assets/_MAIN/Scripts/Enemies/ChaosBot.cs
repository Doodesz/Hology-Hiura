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

    [Header("Variables")]
    [SerializeField] float spotValue;
    [SerializeField] float unawareSpotTime = 2f;
    [SerializeField] float searchingSpotTime = 1f;
    [SerializeField] float spottingRate = 1f;
    [SerializeField] float despottingRate = 1f;
    [SerializeField] float unawareSpottingTimeout = 3f;
    public float speed = 10f;

    [Header("Debugging")]
    public bool playerSpotted;
    [SerializeField] ChaosBotState currState;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        /*// Increase/decrease spot bar when inside/outside of view
        if (fov.canSeePlayer)
        {
            spotValue += spottingRate * Time.deltaTime;
            transform.LookAt(fov.target.gameObject.transform.position);
        }
        else
            spotValue -= despottingRate * Time.deltaTime;

        if (spotValue >= unawareSpotTime)
            playerSpotted = true;
        else
            playerSpotted = false;
*/
    }
}
