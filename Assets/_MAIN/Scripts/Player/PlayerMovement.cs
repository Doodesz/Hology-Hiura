using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(ControllableElectronic))]
public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Rigidbody rb;
    [SerializeField] Transform orientation;
    [SerializeField] Animator anim;
    [SerializeField] GameObject systemsOfflineUI;
    [SerializeField] ControllableElectronic electronicScript;
    [SerializeField] ParticleSystem particle;
    [SerializeField] RepairElectronic repair;
    [SerializeField] MainIngameUI mainIngameUI;

    [Header("Movement")]
    public float moveSpeed;
    public float groundDrag;

    [HideInInspector] public float walkSpeed;
    [HideInInspector] public float sprintSpeed;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;

    [Header("Debugging")]
    [SerializeField] bool grounded;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;

    ElectronicType thisElectronicType;

    PlayerController playerController;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        playerController = PlayerController.Instance;
        mainIngameUI = MainIngameUI.Instance;

        thisElectronicType = GetComponent<ControllableElectronic>().thisElectronicType;
    }

    private void Update()
    {
        // ground check
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight);
        if (thisElectronicType == ElectronicType.Humanoid)
        {
            if (grounded)
                anim.SetBool("isFalling", false);
            else
                anim.SetBool("isFalling", true);
        }

        MyInput();
        SpeedControl();

        rb.drag = groundDrag;
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void MyInput()
    {
        if (playerController.currPlayerObj == gameObject && electronicScript.isOnline)
        {
            horizontalInput = Input.GetAxisRaw("Horizontal");
            verticalInput = Input.GetAxisRaw("Vertical");

            if (horizontalInput != 0 || verticalInput != 0)
            {
                if (thisElectronicType == ElectronicType.Humanoid)
                    anim.SetBool("isWalking", true);
            }
            else if (thisElectronicType == ElectronicType.Humanoid)
            {
                anim.SetBool("isWalking", false);
            }
        }

        else
        {
            horizontalInput = 0;
            verticalInput = 0;

            if (thisElectronicType == ElectronicType.Humanoid)
                anim.SetBool("isWalking", false);
        }

        // For checking curr speed
/*        if (rb.velocity.magnitude != 0f)
            Debug.Log(rb.velocity.magnitude);*/
    }

    private void MovePlayer()
    {
        // calculate movement direction
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        // limit velocity if needed
        if (flatVel.magnitude > moveSpeed / 3)
        {
            Vector3 limitedVel = flatVel.normalized * (moveSpeed/3);
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }

    public void DisableMovement()
    {
        systemsOfflineUI.SetActive(true);
        electronicScript.isOnline = false;
        anim.SetBool("isOnline", false);
        particle.gameObject.SetActive(true);
        repair.canFix = true;
    }

    public void EnableMovement()
    {
        systemsOfflineUI.SetActive(false);
        electronicScript.isOnline = true;
        //anim.SetBool("isOnline", true);       player not in control
        particle.gameObject.SetActive(false);
        repair.canFix = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Triggers pushing anim
        if (other.gameObject.CompareTag("Heavy") && thisElectronicType == ElectronicType.Humanoid
            && other.GetComponent<ChaosBot>() == null && other.GetComponent<PlayerMovement>() == null)
            anim.SetBool("isPushing", true);

        // Show repair prompt when near a disabled electronic
        if (other.gameObject.layer == 7 && TryGetComponent<PlayerMovement>(out PlayerMovement otherMovement)
            && !otherMovement.electronicScript.isOnline && electronicScript.isOnline
            && playerController.currPlayerObj == gameObject)
        {
            otherMovement.repair.canFix = true;
            mainIngameUI.gameObject.GetComponent<Animator>().SetBool("showPrompt", true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Untriggers pushing anim
        if (other.gameObject.CompareTag("Heavy") && thisElectronicType == ElectronicType.Humanoid
            && other.GetComponent<ChaosBot>() == null && other.GetComponent<PlayerMovement>() == null)
            anim.SetBool("isPushing", false);

        // Show repair prompt when near a disabled electronic
        if (other.gameObject.layer == 7 && TryGetComponent<PlayerMovement>(out PlayerMovement otherMovement)
            && !otherMovement.electronicScript.isOnline && electronicScript.isOnline)
        {
            otherMovement.repair.canFix = false;
            mainIngameUI.gameObject.GetComponent<Animator>().SetBool("showPrompt", false);
        }
    }
}