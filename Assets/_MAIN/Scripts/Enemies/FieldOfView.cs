using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    [Header("References")]
    public GameObject raySource;

    [Header("Parameters")]
    public float radius;
    [Range(0, 360)]
    public float angle;

    [Header("Masks")]
    public LayerMask targetMask;
    public LayerMask obstructionMask;

    [Header("Debugging")]
    public GameObject lastTarget = null;
    public bool canSeePlayerElectronic;

    private void Start()
    {
        StartCoroutine(FOVRoutine());
    }

    private IEnumerator FOVRoutine()
    {
        WaitForSeconds wait = new WaitForSeconds(0.15f);

        while (true)
        {
            yield return wait;
            FieldOfViewCheck();
        }
    }

    private void FieldOfViewCheck()
    {
        Vector3 originPoint = new Vector3(raySource.transform.position.x, 
            raySource.transform.position.y - 1.7f, raySource.transform.position.z);
        Collider[] rangeChecks = Physics.OverlapSphere(originPoint, radius, targetMask);

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
            foreach (GameObject obj in targets)
            {
                Vector3 directionToTarget = (obj.transform.position - transform.position).normalized;

                if (Vector3.Angle(transform.forward, directionToTarget) < angle / 2)
                {
                    float distanceToTarget = Vector3.Distance(transform.position, obj.transform.position);

                    if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obstructionMask)
                        && obj.layer == 7 && obj.transform.position.y < transform.position.y + 10f
                        && obj.TryGetComponent<ControllableElectronic>(out ControllableElectronic script)
                        && script.isOnline == true)
                    {

                        canSeePlayerElectronic = true;
                        lastTarget = obj.gameObject;

                        break;
                    }
                    else
                        canSeePlayerElectronic = false;
                }
                else
                    canSeePlayerElectronic = false;
            }
        }
        else if (canSeePlayerElectronic)
            canSeePlayerElectronic = false;
    }
}