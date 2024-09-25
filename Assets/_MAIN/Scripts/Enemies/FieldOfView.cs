using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    public float radius;
    [Range(0, 360)]
    public float angle;

    public GameObject target = null;
    
    public LayerMask targetMask;
    public LayerMask obstructionMask;

    public bool canSeePlayer;

    private void Start()
    {
        StartCoroutine(FOVRoutine());
    }

    private IEnumerator FOVRoutine()
    {
        WaitForSeconds wait = new WaitForSeconds(0.2f);

        while (true)
        {
            yield return wait;
            FieldOfViewCheck();
        }
    }

    private void FieldOfViewCheck()
    {
        Collider[] rangeChecks = Physics.OverlapSphere(transform.position, radius, targetMask);

        // Converts the rangeChecks array to a transform array of that rangeChecks collider game objects
        GameObject[] targets = new GameObject[rangeChecks.Length];
        for (int i = 0; i < rangeChecks.Length; i++)
        {
            if (rangeChecks[i] != null) // Ensure the collider is not null
            {
                targets[i] = rangeChecks[i].gameObject;
            }
        }

        if (rangeChecks.Length != 0)
        {
            foreach (GameObject playerObj in targets)
            {
                Vector3 directionToTarget = (playerObj.transform.position - transform.position).normalized;

                if (Vector3.Angle(transform.forward, directionToTarget) < angle / 2)
                {
                    float distanceToTarget = Vector3.Distance(transform.position, playerObj.transform.position);

                    if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obstructionMask)
                        && playerObj == PlayerController.Instance.currentPlayerObj)
                    {
                        canSeePlayer = true;
                        target = playerObj.gameObject;
                        // Call a attempt to kill player ienumerator function

                        break;
                    }
                    else
                        canSeePlayer = false;
                    
                }
                else
                    canSeePlayer = false;
            }
        }
        else if (canSeePlayer)
            canSeePlayer = false;
    }
}