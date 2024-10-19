using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ControllableElectronic), typeof(PlayerMovement))]
public class RepairElectronic : MonoBehaviour
{
    [Header("References")]
    [SerializeField] PlayerMovement playerMovement;

    [Header("Variables")]
    [SerializeField] float fixValue = 0f;
    [SerializeField] float fixTime = 2f;
    [SerializeField] float fixMultiplier = 1f;
    public bool canFix = false;

    [Header("Debugging")]
    [SerializeField] Animator promptAnim;

    private void Start()
    {
        promptAnim = MainIngameUI.Instance.GetComponent<Animator>(); 
    }

    private void Update()
    {
        if (canFix)
        {
            promptAnim.SetBool("showPrompt", true);

            if (canFix && Input.GetKey(KeyCode.F) && PlayerController.Instance.currPlayerObj.GetComponent<ControllableElectronic>().isOnline)
            {
                fixValue += Time.deltaTime * fixMultiplier;
            }

            if (fixValue >= fixTime && canFix)
            {
                playerMovement.EnableMovement();
                promptAnim.SetBool("showPrompt", false);
                canFix = false; 
            }
        }
    }

    public void InitializeDisabledBehaviour()
    {
        fixValue = 0f;
    }
}
