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
    [SerializeField] float triggerExitTimeout;
    [SerializeField] float triggerExitTimeoutValue;
    [SerializeField] bool hasExitedTrigger;

    private void OnEnable()
    {
        PlayerController.OnSwitchElectronic += ResetVariables;
    }
    private void OnDisable()
    {
        PlayerController.OnSwitchElectronic -= ResetVariables;
    }

    private void Update()
    {
        if (triggerExitTimeoutValue <= 0 && !hasExitedTrigger)
        {
            hasExitedTrigger = true;
            
            platformScript.RemoveLoad(gameObject);     // put first to avoid null ref

            platformScript = null;

            if (PlayerController.Instance.currPlayerObj == thisParent)
            {
                ResetVariables();
            }
            else
            {
                hasPendingReset = true;
            }
        }
        else
            triggerExitTimeoutValue -= Time.deltaTime;
    }

    GameObject CheckRootParent(GameObject transformToCheck)
    {
        // If there's another parent, return that parent
        if (transformToCheck.transform.parent != null)
            return CheckRootParent(transformToCheck.transform.parent.gameObject);
        else // If there's no more parent, return this
            return transformToCheck;
    }

    int FindIndex()
    {
        int i = 0;
        foreach (GameObject obj in platformScript.platformItems)
        {
            if (obj == gameObject)
                return i;
            ++i;
        }
        return i;
    }

    void ResetVariables()
    {
        if (hasPendingReset || PlayerController.Instance.currPlayerObj != thisParent)
        {
            isStandingOnMovingPlatform = false;
            platformParent = null;
            platformScript = null;
            platformItemAnchor = null;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == 16)
        {
            isStandingOnMovingPlatform = true;
            platformParent = CheckRootParent(other.transform.gameObject);
            platformScript = platformParent.GetComponent<DronePlatform>();
            platformScript.AddLoad(gameObject);         // put last to avoid null ref
            platformItemAnchor = platformScript.itemAnchor[FindIndex()];

            hasExitedTrigger = false;
            triggerExitTimeoutValue = 3f;

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

    /*private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 16)
        {

        }
    }*/
}
