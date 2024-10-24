using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class BoxOnPlatformCheck : MonoBehaviour
{
    [SerializeField] DronePlatform platformScript;

    [SerializeField] float triggerExitTimeout;
    [SerializeField] float triggerExitTimeoutValue = 3f;
    [SerializeField] bool hasExitedTrigger = true;

/*    void CheckCurrentCondition()
    {
        if (PlayerController.Instance.currPlayerObj != null && PlayerController.Instance.currPlayerObj == platformScript.gameObject)
        {
            GetComponent<Rigidbody>().useGravity = true;
            GetComponent<GravityModifier>().gravityMultiplier = 1f;
        }
    }*/

    private void Update()
    {
        if (triggerExitTimeoutValue <= 0 && !hasExitedTrigger)
        {
            hasExitedTrigger = true;

            GetComponent<Rigidbody>().useGravity = true;
            GetComponent<GravityModifier>().gravityMultiplier = 1f;

            if (platformScript.platformItems.Contains(gameObject))
                platformScript.RemoveLoad(gameObject);

            platformScript = null;
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

    int FindIndex(DronePlatform platform)
    {
        int i = 0;
        foreach (GameObject obj in platform.platformItems)
        {
            if (obj == gameObject)
                return i;
            ++i;
        }
        return i;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == 16 && CheckRootParent(other.gameObject).TryGetComponent<DronePlatform>(out platformScript))
        {
            if (!platformScript.platformItems.Contains(gameObject))
                platformScript.AddLoad(gameObject);
            
            GetComponent<Rigidbody>().useGravity = false;
            GetComponent<GravityModifier>().gravityMultiplier = 0.1f;
            
            hasExitedTrigger = false;
            triggerExitTimeoutValue = 0.5f;

            if (PlayerController.Instance.currPlayerObj != CheckRootParent(other.gameObject))
            {
                platformScript.itemAnchor[FindIndex(platformScript)].transform.position = transform.position;
                platformScript.itemAnchor[FindIndex(platformScript)].transform.rotation = transform.rotation;
            }
            else
            {
                transform.position = platformScript.itemAnchor[FindIndex(platformScript)].transform.position;
                transform.rotation = platformScript.itemAnchor[FindIndex(platformScript)].transform.rotation;
            }

        }
    }

    /*private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 16 && CheckRootParent(other.gameObject).TryGetComponent<DronePlatform>(out DronePlatform drone))
        {
            GetComponent<Rigidbody>().useGravity = true;
            GetComponent<GravityModifier>().gravityMultiplier = 1f;

            if (drone.platformItems.Contains(gameObject))
                drone.RemoveLoad(gameObject);
        }
    }*/
}
