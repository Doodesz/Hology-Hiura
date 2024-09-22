using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    public GameObject initialPlayerObj;


    [Header("Info")]
    public GameObject currentPlayerObj;

    [SerializeField] RaycastHit rayHitInfo;
    //[SerializeField] Transform currentCam;

    public static PlayerController Instance;

    private void Awake()
    {
        Instance = this;
        
    }

    void Start()
    {

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        SwitchPlayerObject(initialPlayerObj);
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * 100, Color.red, 0.001f, depthTest: true);

        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out rayHitInfo, 100f)
            && rayHitInfo.collider.gameObject.TryGetComponent(out ControllableObject controllableObject) 
            && Input.GetMouseButtonUp(1))
        {
            SwitchPlayerObject(rayHitInfo.collider.gameObject);

            Debug.Log("Switching to " + rayHitInfo.collider.gameObject);
        }
        
        //Debug.Log("Ray hit: " + rayHitInfo.collider.gameObject);
    }

    void SwitchPlayerObject(GameObject objToSwitchTo)
    {
        // Avoids missing reference on scene start
        if (currentPlayerObj != null)                
            currentPlayerObj.GetComponent<ControllableObject>().objCamera.enabled = false;

        currentPlayerObj = objToSwitchTo;

        currentPlayerObj.GetComponent<ControllableObject>().objCamera.enabled = true;

        // Debugging
        Debug.Log("Currently controlling " + currentPlayerObj);
    }
}
