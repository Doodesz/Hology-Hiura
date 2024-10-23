using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatformCheck : MonoBehaviour
{
    [Header("References")]
    [SerializeField] ControllableElectronic electronic;
    [SerializeField] GameObject thisParent;
    [SerializeField] Rigidbody rb;

    [Header("Debugging")]
    public GameObject platformItemAnchor;
    public GameObject platformParent;
    public DronePlatform platformScript;
    public bool isStandingOnMovingPlatform;
    [SerializeField] bool hasPendingReset;

    private void OnEnable()
    {
        PlayerController.OnSwitchElectronic += ResetVariables;
    }
    private void OnDisable()
    {
        PlayerController.OnSwitchElectronic -= ResetVariables;
    }

    GameObject CheckRootParent(GameObject transformToCheck)
    {
        // If there's another parent, return that parent
        if (transformToCheck.transform.parent != null)
            return CheckRootParent(transformToCheck.transform.parent.gameObject);
        else // If there's no more parent, return this
            return transformToCheck;
    }

    void ResetVariables()
    {
        if (hasPendingReset)
        {
            isStandingOnMovingPlatform = false;
            platformParent = null;
            platformScript = null;
            platformItemAnchor = null;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 16 || other.gameObject.layer == 13)
        {
            isStandingOnMovingPlatform = true;
            platformParent = CheckRootParent(other.transform.gameObject);
            platformScript = platformParent.GetComponent<DronePlatform>();
            platformItemAnchor = platformScript.itemAnchor;

            if (!platformScript.platformItems.Contains(gameObject))
                platformScript.AddLoad(gameObject);         // put last to avoid null ref
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == 16 || other.gameObject.layer == 13)
        {
            if (platformScript.platformItems.Count > 1)
            {
                rb.useGravity = true;
            }
            else
            {
                rb.useGravity = false;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 16 || other.gameObject.layer == 13)
        {
            platformScript = CheckRootParent(other.transform.gameObject).GetComponent<DronePlatform>();

            if (platformScript.platformItems.Contains(gameObject))  
                platformScript.RemoveLoad(gameObject);     // put first to avoid null ref
            
            if (PlayerController.Instance.currPlayerObj == thisParent)
            {
                ResetVariables();
            }
            else
            {
                hasPendingReset = true;
            }
        }
    }
}
