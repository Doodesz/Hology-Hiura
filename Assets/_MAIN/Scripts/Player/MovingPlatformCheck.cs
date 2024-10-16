using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatformCheck : MonoBehaviour
{
    [Header("References")]
    [SerializeField] GameObject parent;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 16)
        {
            gameObject.transform.SetParent(other.transform, true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 16)
        {
            gameObject.transform.SetParent(null);
        }
    }
}
