using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlScheme : MonoBehaviour
{
    [SerializeField] Animator anim;
    [SerializeField] float idleValue;
    [SerializeField] float idleTime = 3f;

    private void Update()
    {
        if (Input.anyKey)
            idleValue = 0f;
        else
            idleValue += Time.deltaTime;

        if (idleValue > idleTime)
            anim.SetBool("isIdle", true);
        else
        {
            anim.SetBool("isIdle", false);
        }
    }
}
