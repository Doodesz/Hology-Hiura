using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectiveObject : MonoBehaviour
{
    [SerializeField] bool isCompleted;
    [SerializeField] float timeToComplete;

    [Header("Debugging")]
    [SerializeField] float timeToCompleteValue;
    [SerializeField] bool canBeInteracted;

    private void Update()
    {
        if (canBeInteracted && Input.GetKey(KeyCode.F) && !isCompleted)
        {
            timeToCompleteValue += Time.deltaTime;
        }

        CheckStatus();
    }

    void CheckStatus()
    {
        if (timeToCompleteValue > timeToComplete)
        {
            OnInteractCompleted();
        }
    }

    void OnInteractCompleted()
    {
        isCompleted = true;
        ObjectiveManager.Instance.UpdateNewObjective();
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == 7 && other.gameObject == PlayerController.Instance.currPlayerObj)
        {
            canBeInteracted = true;
        }
        else
        {
            canBeInteracted = false;
        }
    }
}
