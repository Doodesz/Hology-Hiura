using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTPPDirection : MonoBehaviour
{
    [Header("References")]
    public Transform orientation;
    public Transform playerObj;
    public Transform cameraObj;
    
    Rigidbody rb;
    PlayerController playerController;

    [Header("Input Values")]
    public float rotationSpeed;
    //[SerializeField] [Range(-180f, 180f)] float customYRotationValue;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerController = PlayerController.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        // Rotate orientation
        Vector3 viewDir = transform.position - 
            new Vector3(cameraObj.transform.position.x, transform.position.y, cameraObj.transform.position.z);
        orientation.forward = viewDir.normalized;

        // Rotate player obj
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");
        Vector3 inputDir = (orientation.forward * verticalInput) + (orientation.right * horizontalInput);

        if ((inputDir != Vector3.zero) && playerController.currentPlayerObj == gameObject)
        {
            playerObj.forward = Vector3.Slerp(playerObj.forward, inputDir.normalized, Time.deltaTime * rotationSpeed);
        }
    }
}
