using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractManager : MonoBehaviour
{
    public bool canFix;
    public RepairElectronic fixObj;

    public bool canObjective;
    public ObjectiveObject objectiveObject;

    [Header("Debugging")]
    [SerializeField] Animator anim;

    public static InteractManager Instance;

    private void Start()
    {
        Instance = this;

        anim = MainIngameUI.Instance.anim;
    }

    public void SetFixObject(RepairElectronic fixRefObj, bool canFixRef)
    {
        // Return when there's already object that can be fixed
        if (canFix && canFixRef)
            return;

        canFix = canFixRef;
        fixObj = fixRefObj;
    }

    public void SetObjectiveObject(ObjectiveObject objectiveObj, bool canObjectiveRef)
    {
        // Return when there's already object that can be interacted
        if (canObjective && canObjectiveRef)
            return;

        canObjective = canObjectiveRef;
        objectiveObject = objectiveObj;
    }

    private void Update()
    {
        if (canFix)
        {
            anim.SetBool("showPrompt", true);

            if (Input.GetKey(KeyCode.F))
            {
                fixObj.fixValue += Time.deltaTime * fixObj.fixMultiplier;
            }
        }
        else if (canObjective)
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
}
