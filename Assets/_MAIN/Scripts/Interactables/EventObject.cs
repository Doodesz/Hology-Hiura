using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EventObjectType { Door, Laser }
public class EventObject : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Animator anim;

    [Header("Modifiers")]
    public bool canBeRetriggered = false;
    [SerializeField] bool isActivated;
    [SerializeField] EventObjectType objType;

    [Header("Debugging")]
    [SerializeField] bool hasBeenTriggered;

    public void TriggerObjectEvent()
    {
        if (canBeRetriggered || !hasBeenTriggered)
        {
            if (!isActivated)     
                TriggerObject();        
            else if (isActivated)
                UntriggerObject();

            isActivated = !isActivated;
            hasBeenTriggered = true;
        }
    }

    void TriggerObject()
    {
        if (objType == EventObjectType.Door)
            anim.SetBool("isOpen", true);
        else if (objType == EventObjectType.Laser)
            anim.SetBool("isDisabled", true);
    }

    void UntriggerObject()
    {
        if (objType == EventObjectType.Door)
            anim.SetBool("isOpen", false);
        else if (objType == EventObjectType.Laser)
            anim.SetBool("isDisabled", false);
    }
}
