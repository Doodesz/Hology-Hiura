using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum ObjectiveType { Interact, EnterArea, Hold, Exit };

public class ObjectiveObject : MonoBehaviour
{
    [Header("References")]
    public Image objectiveIcon;
    [SerializeField] Image objectiveValueIcon;
    [SerializeField] Computer computer;
    [SerializeField][Tooltip("Assign if this is an exit")] 
        BoxCollider exitCollider;
    [SerializeField][Tooltip("Assign if this is an exit")] 
        Animator anim;
    [SerializeField][Tooltip("Assign if this is an exit")] 
        GameObject exitCube;

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

        // Enables icon if this object is the first in the objective list
        objectiveIcon.enabled = false;
        if (objectiveManager.objectives[0].objectiveObject == this)
            objectiveIcon.enabled = true;

        // Turns this collider to a collision if type is an exit and on the last of the list
        if (objectiveManager.objectives[objectiveManager.objectives.Count-1].objectiveObject == this
            && type == ObjectiveType.Exit)
        {
            exitCollider.isTrigger = false;
        }
        else if (type == ObjectiveType.EnterArea)
        {
            exitCollider.isTrigger = true;
            exitCube.SetActive(false);
        }

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
            computer.OnAfterInteraction();
    }

    public void ChangeColliderToTrigger()
    {
        exitCollider.isTrigger = true;
        anim.SetBool("showWall", false);
    }

    private void OnTriggerStay(Collider other)
    {
        if (!isCompleted && type != ObjectiveType.Hold)
        {
            if ((other.gameObject.layer == 7 || other.gameObject.layer == 17) 
                && other.TryGetComponent<ControllableElectronic>(out ControllableElectronic otherScript)
                && otherScript.gameObject == PlayerController.Instance.currPlayerObj)
            {
                if (type == ObjectiveType.Interact && otherScript.thisElectronicType == ElectronicType.Humanoid
                    && objectiveManager.objectives[objectiveManager.currIndex].objectiveObject == this)
                {
                    canBeInteracted = true;
                    interactManager.SetObjectiveObject(this, true);
                }
                else if (type == ObjectiveType.EnterArea 
                    || (type == ObjectiveType.Exit && objectiveManager.currIndex == objectiveManager.objectives.Count-1))
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

    private void OnCollisionStay(Collision collision)
    {
        if (!isCompleted && (collision.gameObject.layer == 7 || collision.gameObject.layer == 17) 
            && objectiveManager.currIndex != objectiveManager.objectives.Count - 1 
            && PlayerController.Instance.currPlayerObj == collision.gameObject
            && type == ObjectiveType.Exit)
        {
            anim.SetBool("showWall", true);
        }
        else if (type == ObjectiveType.Exit)
        {
            anim.SetBool("showWall", false);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (!isCompleted && (collision.gameObject.layer == 7 || collision.gameObject.layer == 17)
            && objectiveManager.currIndex != objectiveManager.objectives.Count - 1
            && PlayerController.Instance.currPlayerObj == collision.gameObject)
        {
            anim.SetBool("showWall", false);
        }
    }
}
