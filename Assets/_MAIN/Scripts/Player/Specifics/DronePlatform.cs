using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DronePlatform : MonoBehaviour
{
    [Header("References")]
    [SerializeField] PlayerMovement movement;
    public GameObject itemAnchor;
    public bool isCarrying;
    public List<Collider> platformColliders;

    [Header("Debugging")]
    public List<GameObject> platformItems = new List<GameObject>();

    private void OnEnable()
    {
        PlayerController.OnSwitchElectronic += TogglePlatformColliders;
    }
    private void OnDisable()
    {
        PlayerController.OnSwitchElectronic -= TogglePlatformColliders;
    }

    public void AddLoad(GameObject obj)
    {
        platformItems.Add(obj);
        CheckCurrentLoad();
    }

    public void RemoveLoad(GameObject obj)
    {
        platformItems.Remove(obj);
        CheckCurrentLoad();
    }

    void CheckCurrentLoad()
    {
        if (platformItems.Count > 0)
            isCarrying = true;

        if (platformItems.Count > 1)
        {
            movement.isOverweighted = true;
            // Overloaded and starts to descend
        }
        else
        {
            movement.isOverweighted = false;
        }
    }

    void TogglePlatformColliders()
    {
        if (PlayerController.Instance.currPlayerObj == gameObject)
        {
            foreach(Collider collider in platformColliders)
            {
                collider.gameObject.SetActive(false);
            }
        }
        else
        {
            foreach(Collider collider in platformColliders)
            {
                collider.gameObject.SetActive(true);
            }
        }
    }
}
