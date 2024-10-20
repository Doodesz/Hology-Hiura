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

        UpdateIcons(false, 0f);
    }

    private void Update()
    {
        if (canFix)
        {
            interactManager.SetFixObject(this, true);
            UpdateIcons(true, fixValue);
        }
        else
        {
            interactManager.SetFixObject(null, false);
            UpdateIcons(false, fixValue);
        }

        if (fixValue >= fixTime && canFix)
        {
            playerMovement.EnableMovement();
            canFix = false;
            UpdateIcons(false, 0f);

            interactManager.SetFixObject(null, false);
        }
    }

    public void InitializeDisabledBehaviour()
    {
        fixValue = 0f;
    }

    void UpdateIcons(bool enable, float progressBar)
    {
        fixIcon.enabled = enable;
        fixValueIcon.fillAmount = progressBar / fixTime;
    }
}
