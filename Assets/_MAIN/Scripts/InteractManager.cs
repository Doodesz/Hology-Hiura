using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractManager : MonoBehaviour
{
    public bool canFix;
    public RepairElectronic fixObj;
    public List<RepairElectronic> reparableElectronics = new List<RepairElectronic>();
    public bool noElectronicsCanBeFixed;

    public bool canObjective;
    public ObjectiveObject objectiveObject;
    public List<ObjectiveObject> objectiveObjects = new List<ObjectiveObject>();
    public bool noObjectivesCanBeInteracted;

    [Header("Debugging")]
    [SerializeField] Animator anim;
    [SerializeField] PlayerController playerController;

    public static InteractManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    { 
        playerController = PlayerController.Instance;

        // Adds all controllable electronics and objectives
        foreach (RepairElectronic electronic in FindObjectsOfType(typeof(RepairElectronic)))
        {
            reparableElectronics.Add(electronic);
        }
        foreach (ObjectiveObject objectiveObject in FindObjectsOfType(typeof(ObjectiveObject)))
        {
            objectiveObjects.Add(objectiveObject);
        }

        anim = MainIngameUI.Instance.anim;
    }

    public void SetFixObject(RepairElectronic fixRefObj, bool canFixRef)
    {
        // Return when there's already object that can be fixed
        if (canFix && canFixRef)
            return;

        canFix = canFixRef;
        fixObj = fixRefObj;

        CheckIfNoElectronicsCanBeFixed();
    }

    public void SetObjectiveObject(ObjectiveObject objectiveObj, bool canObjectiveRef)
    {
        // Return when there's already object that can be interacted
        if (canObjective && canObjectiveRef)
            return;

        canObjective = canObjectiveRef;
        objectiveObject = objectiveObj;
    }

    public void CheckIfNoElectronicsCanBeFixed()
    {
        foreach (RepairElectronic electronic in reparableElectronics)
        {
            if (electronic.canBeFixed)
            {
                noElectronicsCanBeFixed = false;
            }
        }

        noElectronicsCanBeFixed = true;
    }

    public void CheckIfNoObjectivesCanBeInteracted()
    {
        foreach (ObjectiveObject obj in objectiveObjects)
        {
            if (obj.canBeInteracted)
            {
                noObjectivesCanBeInteracted = false;
            }
        }

        noObjectivesCanBeInteracted = true;
    }

    public bool NoElectronicsCanBeFixed()
    {
        foreach (RepairElectronic electronic in reparableElectronics)
        {
            if (electronic.canBeFixed)
            {
                noElectronicsCanBeFixed = false;
                return false;
            }
        }

        noElectronicsCanBeFixed = true;
        return true;
    }

    public bool NoObjectivesCanBeInteracted()
    {
        foreach (ObjectiveObject obj in objectiveObjects)
        {
            if (obj.canBeInteracted)
            {
                noObjectivesCanBeInteracted = false;
                return false;
            }
        }

        noObjectivesCanBeInteracted = true;
        return true;
    }

    private void Update()
    {
        if (playerController.currPlayerObj.GetComponent<ControllableElectronic>().isOnline)
        {
            if (canFix)
            {
                anim.SetBool("showPrompt", true);

                if (Input.GetKey(KeyCode.F))
                {
                    fixObj.fixValue += Time.deltaTime * fixObj.fixMultiplier;
                }
            }
            else if (canObjective 
                && playerController.currPlayerObj.GetComponent<ControllableElectronic>().thisElectronicType == ElectronicType.Humanoid)
            {
                anim.SetBool("showPrompt", true);

                if (Input.GetKey(KeyCode.F))
                {
                    objectiveObject.timeToCompleteValue += Time.deltaTime;
                }
            }
            else
            {
                anim.SetBool("showPrompt", false);
            }
        }
        else
        {
            anim.SetBool("showPrompt", false);
        }
    }
}
