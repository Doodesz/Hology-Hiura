using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    [Header("References")]
    [SerializeField] List<EventObject> objectsToTrigger = new List<EventObject>();
    [Header("Debugging")]
    [SerializeField] List<GameObject> objectsOnPlate = new List<GameObject>();
    [SerializeField] bool isPressed;

    Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void TriggerEvent()
    {
        Debug.Log("Pressure plate hit");
        // play sfx
        foreach (EventObject obj in objectsToTrigger)
        {
            obj.TriggerObjectEvent();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Heavy"))
        {
            if (!isPressed)
            {
                isPressed = true;
                TriggerEvent();
                if (gameObject.activeInHierarchy)
                    animator.SetBool("pressed", true);
            }

            // Adds current object to a list of objects pressing the plate
            objectsOnPlate.Add(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Heavy"))
        {
            // Removes exited object from the list of objects pressing the plate
            for (int i = 0; i < objectsOnPlate.Count; i++)
            {
                if (objectsOnPlate[i] == other.gameObject) objectsOnPlate.Remove(objectsOnPlate[i]);
            }

            // If no box or player is on the plate, release the plate
            if (objectsOnPlate.Count == 0)
            {
                TriggerEvent();
                if (gameObject.activeInHierarchy)
                    animator.SetBool("pressed", false);
                isPressed = false;
            }
        }
        
    }
}
