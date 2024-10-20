using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ObjectiveType { Interact, Exit  };

public class ObjectiveObject : MonoBehaviour
{
    [SerializeField] bool isCompleted;
    [SerializeField] float timeToComplete;
    [SerializeField] ObjectiveType type;

    [Header("Debugging")]
    public float timeToCompleteValue;
    [SerializeField] bool canBeInteracted;
    [SerializeField] ObjectiveManager objectiveManager;
    [SerializeField] InteractManager interactManager;

    private void Start()
    {
        objectiveManager = ObjectiveManager.Instance;
        interactManager = InteractManager.Instance;
    }

    private void Update()
    {
        if (canBeInteracted && Input.GetKey(KeyCode.F) && !isCompleted)
        {
            timeToCompleteValue += Time.deltaTime;
            CheckStatus();
        }

    }

    void CheckStatus()
    {
        if (timeToCompleteValue > timeToComplete && !isCompleted)
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
        if (!isCompleted)
        {
            if (other.gameObject.layer == 7 && other.TryGetComponent<ControllableElectronic>(out ControllableElectronic otherScript)
                && otherScript.gameObject == PlayerController.Instance.currPlayerObj
                && objectiveManager.objectives[objectiveManager.currIndex].objectiveObject == this)
            {
                if (type == ObjectiveType.Interact)
                {
                    canBeInteracted = true;
                    interactManager.SetObjectiveObject(this, true);
                }
                else if (type == ObjectiveType.Exit)
                {
                    OnInteractCompleted();
                }
            }
            else
            {
                if (type == ObjectiveType.Interact)
                {
                    canBeInteracted = false;
                    interactManager.SetObjectiveObject(null, false);
                }

            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!isCompleted)
        {
            if (other.gameObject.layer == 7 && other.TryGetComponent<ControllableElectronic>(out ControllableElectronic otherScript)
                && otherScript.gameObject == PlayerController.Instance.currPlayerObj
                && objectiveManager.objectives[objectiveManager.currIndex].objectiveObject == this)
            {
                if (type == ObjectiveType.Interact)
                {
                    canBeInteracted = false;
                    interactManager.SetObjectiveObject(null, false);
                }
            }

        }
    }
}
