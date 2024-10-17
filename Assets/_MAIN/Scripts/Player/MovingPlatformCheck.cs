using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatformCheck : MonoBehaviour
{
    [Header("References")]
    public GameObject platformItemAnchor;

    [Header("Debugging")]
    public GameObject platformParent;
    public bool isStandingOnMovingPlatform;

    GameObject CheckRootParent(GameObject transformToCheck)
    {
        // If there's another parent, return that parent
        if (transformToCheck.transform.parent != null)
            return CheckRootParent(transformToCheck.transform.parent.gameObject);
        else // If there's no more parent, return this
            return transformToCheck;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 16)
        {
            //thisParent.transform.SetParent(CheckRootParent(other.transform.gameObject).transform, true);
            platformParent = CheckRootParent(other.transform.gameObject);
            isStandingOnMovingPlatform = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 16)
        {
            //thisParent.transform.SetParent(null);
            platformParent = null;
            isStandingOnMovingPlatform = false;
        }
    }
}
