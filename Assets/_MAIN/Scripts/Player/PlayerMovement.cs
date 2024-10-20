using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(ControllableElectronic), typeof(ObjectSoundManager))]
public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Transform orientation;
    [SerializeField] Animator anim;
    [SerializeField] GameObject systemsOfflineUI;
    [SerializeField] ControllableElectronic electronicScript;
    [SerializeField] ParticleSystem particle;
    [SerializeField] RepairElectronic repair;
    [SerializeField] ObjectSoundManager soundManager;

    [Header("Conditional Ref")]
    [SerializeField][Tooltip("Assign if rb is not in parent")] 
        Rigidbody rb;

    [Header("Movement")]
    public float moveSpeed;
    public float movementDrag;
    [SerializeField] bool isPushing;

    [HideInInspector] public float walkSpeed;
    [HideInInspector] public float sprintSpeed;

    [Header("Ground Check")]
    [Tooltip("Only to check if falling for humanoid")] 
        public float playerHeight;
    public LayerMask whatIsGround;

    [Header("Debugging")]
    [SerializeField] bool grounded;
    [SerializeField] MainIngameUI mainIngameUI;
    [SerializeField] InteractManager interactManager;
    public bool isOverweighted;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;

    ElectronicType thisElectronicType;

    PlayerController playerController;

    private void Start()
    {
        // If not assigned but component is in gameObject, assign that
        if (rb == null && TryGetComponent<Rigidbody>(out Rigidbody rbRef))
            rb = rbRef;
        rb.freezeRotation = true;

        playerController = PlayerController.Instance;
        mainIngameUI = MainIngameUI.Instance;
        interactManager = InteractManager.Instance;

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

        if (isOverweighted)
            DescendSlowly();

        MoveInput();
        SpeedControl();

        rb.drag = movementDrag;
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void MoveInput()
    {
        if (playerController.currPlayerObj == gameObject && electronicScript.isOnline)
        {
            horizontalInput = Input.GetAxisRaw("Horizontal");
            verticalInput = Input.GetAxisRaw("Vertical");

            // If this object is humanoid (minibot), set anim bool
            if (horizontalInput != 0 || verticalInput != 0)
            {
                // If this is a humanoid (minibot)
                if (thisElectronicType == ElectronicType.Humanoid)
                {
                    anim.SetBool("isWalking", true);
                }

                // Play move sfx
                if (!soundManager.moveSfx.isPlaying)
                    soundManager.PlayMove();
            }
            else if (thisElectronicType == ElectronicType.Humanoid)
            {
                anim.SetBool("isWalking", false);

                // Pause move sfx
                if (soundManager.moveSfx.isPlaying)
                    soundManager.PauseMove();
            }

            // If this is a drone
            if (thisElectronicType == ElectronicType.Drone)
            {
                if (Input.GetKey(KeyCode.Q))
                    Descend();
                if (Input.GetKey(KeyCode.E))
                    Ascend();
            }
        }

        else
        {
            horizontalInput = 0;
            verticalInput = 0;

            // If this object is humanoid, set anim bool
            if (thisElectronicType == ElectronicType.Humanoid)
                anim.SetBool("isWalking", false);

            // Pause move sfx
            if (soundManager.moveSfx.isPlaying)
                soundManager.PauseMove();
        }

        // For checking curr speed
/*        if (rb.velocity.magnitude != 0f)
            Debug.Log(rb.velocity.magnitude);*/
    }

    private void MovePlayer()
    {
        // calculate movement direction
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        {
            rb.AddRelativeForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
        }
    }

    private void Ascend()
    {
        rb.AddRelativeForce(Vector3.up * moveSpeed / 2 * 10f, ForceMode.Force);
    }

    private void Descend()
    {
        rb.AddRelativeForce(Vector3.down * moveSpeed / 2 * 10f, ForceMode.Force);
    }

    private void DescendSlowly()
    {
        rb.AddRelativeForce(Vector3.down * moveSpeed / 4 * 10f, ForceMode.Force);
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
        soundManager.PlayDisabled();
    }

    public void EnableMovement()
    {
        systemsOfflineUI.SetActive(false);
        electronicScript.isOnline = true;
        //anim.SetBool("isOnline", true);       player not in control
        particle.gameObject.SetActive(false);
        soundManager.PauseDisabled();
    }

    private void OnTriggerEnter(Collider other)
    {
        // Triggers pushing anim
        if (other.gameObject.CompareTag("Heavy") && thisElectronicType == ElectronicType.Humanoid
            && other.GetComponent<ChaosBot>() == null && other.GetComponent<PlayerMovement>() == null)
        {
            anim.SetBool("isPushing", true);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        // Show repair prompt when near a disabled electronic
        if (other.gameObject.layer == 7 && other.TryGetComponent<PlayerMovement>(out PlayerMovement otherMovement)
            && !otherMovement.electronicScript.isOnline && electronicScript.isOnline
            && playerController.currPlayerObj == gameObject)
        {
            otherMovement.repair.canBeFixed = true;
        }
        else if (other.gameObject.layer == 7 && other.TryGetComponent<PlayerMovement>(out PlayerMovement otherMovement1)
            && playerController.currPlayerObj != gameObject)
        {
            otherMovement1.repair.canBeFixed = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Untriggers pushing anim
        if (other.gameObject.CompareTag("Heavy") && thisElectronicType == ElectronicType.Humanoid
            && other.GetComponent<ChaosBot>() == null && other.GetComponent<PlayerMovement>() == null)
        {
            anim.SetBool("isPushing", false);
        }

        // Show repair prompt when near a disabled electronic
        if (other.gameObject.layer == 7 && other.TryGetComponent<PlayerMovement>(out PlayerMovement otherMovement)
            && !otherMovement.electronicScript.isOnline && electronicScript.isOnline
            && playerController.currPlayerObj == gameObject)
        {
            otherMovement.repair.canBeFixed = false;
            interactManager.CheckIfNoElectronicsCanBeFixed();
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        // Triggers pushing anim
        if (other.gameObject.CompareTag("Heavy") && thisElectronicType == ElectronicType.Humanoid
            && other.gameObject.GetComponent<ChaosBot>() == null && other.gameObject.GetComponent<PlayerMovement>() == null)
        {
            if (!soundManager.pushSfx.isPlaying)
                soundManager.PlayPush();
        }
    }

    private void OnCollisionExit(Collision other)
    {
        // Untriggers pushing anim
        if (other.gameObject.CompareTag("Heavy") && thisElectronicType == ElectronicType.Humanoid
            && other.gameObject.GetComponent<ChaosBot>() == null && other.gameObject.GetComponent<PlayerMovement>() == null)
        {
            if (soundManager.pushSfx.isPlaying)
                soundManager.PausePush();
        }
    }
}