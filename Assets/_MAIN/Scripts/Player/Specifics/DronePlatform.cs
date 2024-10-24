using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DronePlatform : MonoBehaviour
{
    [Header("References")]
    [SerializeField] PlayerMovement movement;
    public List<GameObject> itemAnchor;
    public bool isCarrying;
    public List<Collider> platformColliders;

    [Header("Debugging")]
    public List<GameObject> platformItems = new List<GameObject>();

    public void AddLoad(GameObject obj)
    {
        if (!platformItems.Contains(obj))
            platformItems.Add(obj);
        CheckCurrentLoad();
    }

    public void RemoveLoad(GameObject obj)
    {
        if (platformItems.Contains(obj))
            platformItems.Remove(obj);
        CheckCurrentLoad();
    }

    void CheckCurrentLoad()
    {
        if (platformItems.Count > 0)
            isCarrying = true;
        if (platformItems.Count <= 0)
            isCarrying = false;

        if (platformItems.Count > 1)
        {
            movement.isOverweighted = true;
        }
        else
        {
            movement.isOverweighted = false;
        }
    }
}
