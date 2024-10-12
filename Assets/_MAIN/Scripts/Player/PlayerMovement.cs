using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(ControllableElectronic))]
public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    public Rigidbody rb;
    public Transform orientation;
    public Animator anim;
    public GameObject systemsOfflineUI;
    public ControllableElectronic electronicScript;

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

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Heavy") && thisElectronicType == ElectronicType.Humanoid
            && other.GetComponent<ChaosBot>() == null && other.GetComponent<PlayerMovement>() == null)
            anim.SetBool("isPushing", true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Heavy") && thisElectronicType == ElectronicType.Humanoid
            && other.GetComponent<ChaosBot>() == null && other.GetComponent<PlayerMovement>() == null)
            anim.SetBool("isPushing", false);
    }
}