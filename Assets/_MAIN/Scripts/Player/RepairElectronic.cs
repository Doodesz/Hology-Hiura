using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ControllableElectronic), typeof(PlayerMovement))]
public class RepairElectronic : MonoBehaviour
{
    [Header("References")]
    [SerializeField] PlayerMovement playerMovement;
    [SerializeField] Image fixIcon;
    [SerializeField] Image fixValueIcon;
    [SerializeField] ControllableElectronic electronic;

    [Header("Variables")]
    public float fixValue = 0f;
    [SerializeField] float fixTime = 2f;
    public float fixMultiplier = 1f;
    public bool canBeFixed = false;

    [Header("Debugging")]
    [SerializeField] Animator promptAnim;
    [SerializeField] InteractManager interactManager;

    private void Start()
    {
        promptAnim = MainIngameUI.Instance.GetComponent<Animator>();
        interactManager = InteractManager.Instance;

        UpdateIcons(false, 0f);
    }

    private void Update()
    {
        if (electronic.isOnline)
            UpdateIcons(false, 0f);
        else
            UpdateIcons(true, fixValue);

        if (canBeFixed)
        {
            interactManager.SetFixObject(this, true);
        }
        else if (interactManager.NoElectronicsCanBeFixed())
        {
            interactManager.SetFixObject(null, false);
        }

        if (fixValue >= fixTime && canBeFixed)
        {
            CompleteElectronicRepair();
        }
    }

    public void InitializeDisabledBehaviour()
    {
        fixValue = 0f;
    }

    public void CompleteElectronicRepair()
    {
        playerMovement.EnableMovement();
        canBeFixed = false;
        fixValue = 0f;

        interactManager.SetFixObject(null, false);
    }

    public void UpdateIcons(bool enable, float progressBar)
    {
        fixIcon.enabled = enable;
        fixValueIcon.fillAmount = progressBar / fixTime;
    }
}
