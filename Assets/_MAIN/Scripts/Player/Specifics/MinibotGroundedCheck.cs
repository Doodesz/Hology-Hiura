using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinibotGroundedCheck : MonoBehaviour
{
    [SerializeField] LayerMask whatIsGround;
    [SerializeField] Animator anim;
    [SerializeField] bool isFalling;

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer != whatIsGround)
        {
            anim.SetBool("isFalling", false);
            isFalling = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer != whatIsGround)
        {
            anim.SetBool("isFalling", true);
            isFalling = true;
        }
    }
}
