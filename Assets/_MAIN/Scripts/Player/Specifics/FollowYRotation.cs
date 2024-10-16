using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowYRotation : MonoBehaviour
{
    [Header("References")]
    [SerializeField] GameObject objectToFollow;

    // Update is called once per frame
    void Update()
    {
        transform.rotation = objectToFollow.transform.rotation;
    }
}
