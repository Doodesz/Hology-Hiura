using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ControllableElectronic), typeof(PlayerMovement))]
public class RepairElectronic : MonoBehaviour
{
    [Header("References")]
    [SerializeField] PlayerMovement playerMovement;

    [Header("Variables")]
    public float fixValue = 0f;
    [SerializeField] float fixTime = 2f;
    public float fixMultiplier = 1f;
    public bool canFix = false;

    [Header("Debugging")]
    [SerializeField] Animator promptAnim;
    [SerializeField] InteractManager interactManager;

    private void Start()
    {
        promptAnim = MainIngameUI.Instance.GetComponent<Animator>();
        interactManager = InteractManager.Instance;
    }

    private void Update()
    {
        /*if (canFix && Input.GetKey(KeyCode.F) && PlayerController.Instance.currPlayerObj.GetComponent<ControllableElectronic>().isOnline)
        {
            fixValue += Time.deltaTime * fixMultiplier;
        }*/

        if (fixValue >= fixTime && canFix)
        {
            playerMovement.EnableMovement();
            //promptAnim.SetBool("showPrompt", false);
            canFix = false;

            interactManager.SetFixObject(null, false);
        }
    }

    public void InitializeDisabledBehaviour()
    {
        fixValue = 0f;
    }
}
