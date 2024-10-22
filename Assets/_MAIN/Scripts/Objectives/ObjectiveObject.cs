using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum ObjectiveType { Interact, Exit, Hold };

public class ObjectiveObject : MonoBehaviour
{
    [Header("References")]
    public Image objectiveIcon;
    [SerializeField] Image objectiveValueIcon;
    [SerializeField] Computer computer;

    [Header("Variables")]
    public bool isCompleted;
    [SerializeField] float timeToComplete;
    [SerializeField] ObjectiveType type;

    [Header("Debugging")]
    public float timeToCompleteValue;
    public bool canBeInteracted;
    [SerializeField] ObjectiveManager objectiveManager;
    [SerializeField] InteractManager interactManager;

    private void Start()
    {
        objectiveManager = ObjectiveManager.Instance;
        interactManager = InteractManager.Instance;

        objectiveIcon.enabled = false;
        if (objectiveManager.objectives[0].objectiveObject == this)
            objectiveIcon.enabled = true;

        objectiveValueIcon.fillAmount = 0;
    }

    private void Update()
    {
        if (canBeInteracted && Input.GetKey(KeyCode.F) && !isCompleted && type != ObjectiveType.Hold)
        {
            //timeToCompleteValue += Time.deltaTime;
            CheckStatus();
        }
    }

    void CheckStatus()
    {
        objectiveValueIcon.fillAmount = timeToCompleteValue / timeToComplete;
        
        if (timeToCompleteValue > timeToComplete && !isCompleted)
        {
            OnInteractCompleted();
        }
    }

    void OnInteractCompleted()
    {
        isCompleted = true;
        objectiveManager.UpdateNewObjective();

        interactManager.SetObjectiveObject(null, false);

        objectiveIcon.enabled = false;
        objectiveValueIcon.fillAmount = 0f;

        if (type == ObjectiveType.Interact)
            computer.PlayVideo();
    }

    private void OnTriggerStay(Collider other)
    {
        if (!isCompleted && type != ObjectiveType.Hold)
        {
            if ((other.gameObject.layer == 7 || other.gameObject.layer == 17) 
                && other.TryGetComponent<ControllableElectronic>(out ControllableElectronic otherScript)
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
            else if (other.TryGetComponent<ControllableElectronic>(out ControllableElectronic otherScript1) 
                && (otherScript1.thisElectronicType == ElectronicType.Roomba
                || otherScript1.thisElectronicType == ElectronicType.Drone))
            {
                // hint can only use minibot
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

        else if (type == ObjectiveType.Hold && (other.gameObject.layer == 7 || other.gameObject.layer == 13))
        {
                isCompleted = true;
                objectiveManager.UpdateNewObjective();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 7 && other.TryGetComponent<ControllableElectronic>(out ControllableElectronic otherScript)
            && otherScript.gameObject == PlayerController.Instance.currPlayerObj
            && objectiveManager.objectives[objectiveManager.currIndex].objectiveObject == this
            && type != ObjectiveType.Hold)
        {
            if (type == ObjectiveType.Interact)
            {
                canBeInteracted = false;
                interactManager.SetObjectiveObject(null, false);
            }
        }

        else if (type == ObjectiveType.Hold && (other.gameObject.layer == 7 || other.gameObject.layer == 13))
        {
            isCompleted = false;
            objectiveManager.UpdateNewObjective();
        }
    }
}
