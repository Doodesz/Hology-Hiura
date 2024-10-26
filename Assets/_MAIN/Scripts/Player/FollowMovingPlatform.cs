using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowMovingPlatform : MonoBehaviour
{
    [Header("References")]
    [SerializeField] MovingPlatformCheck check;
    [SerializeField] GameObject objToRotate;

    [Header("Debugging")]
    [SerializeField] GameObject objectToFollow;

    void Update()
    {
        // If this is a bot
        if (check != null && check.isStandingOnMovingPlatform)
        {
            objectToFollow = check.platformItemAnchor;

            // Update anchor when not controlling drone
            if (PlayerController.Instance.currPlayerObj == gameObject)
            {
                objectToFollow.transform.position = gameObject.transform.position;
                objectToFollow.transform.rotation = objToRotate.transform.rotation;
            }
            else // Follows drone platform when controlling said drone
            {
                gameObject.transform.position = objectToFollow.transform.position + Vector3.up * 0.2f;
                objToRotate.transform.rotation = objectToFollow.transform.rotation;
            }
        }
    }
}
