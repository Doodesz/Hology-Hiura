using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowMovingPlatform : MonoBehaviour
{
    [Header("References")]
    [SerializeField] MovingPlatformCheck check;

    [Header("Debugging")]
    [SerializeField] GameObject objectToFollow;

    void Update()
    {
        if (check.isStandingOnMovingPlatform)
            objectToFollow = check.platformItemAnchor;

        if (check.isStandingOnMovingPlatform && PlayerController.Instance.currPlayerObj == gameObject)
        {
            objectToFollow.transform.position = gameObject.transform.position;
        }
        else if (check.isStandingOnMovingPlatform)
        {
            gameObject.transform.position = objectToFollow.transform.position;
        }
    }
}
