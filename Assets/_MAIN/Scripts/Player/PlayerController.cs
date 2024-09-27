using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    public GameObject initialPlayerObj;


    [Header("Info")]
    public GameObject currPlayerObj;

    [SerializeField] RaycastHit rayHitInfo;

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
    }

    void SwitchPlayerObject(GameObject objToSwitchTo)
    {
        // Assigns first player object when scene starts
        if (currPlayerObj == null)
        {
            currPlayerObj = initialPlayerObj;
        }

        ControllableObject currPlayerObjScript = currPlayerObj.GetComponent<ControllableObject>();
        CinemachineFreeLook currPlayerObjCam = currPlayerObjScript.objCamera;

        // Disables old cam
        currPlayerObjCam.enabled = false;

        // Sets offline animation on switched off minibot
        if (currPlayerObjScript.thisElectronicType == ElectronicType.Humanoid)
            currPlayerObjScript.animator.SetBool("isOnline", false);
        
        // Switches controlled object
        currPlayerObj = objToSwitchTo;

        // Updates local variables
        currPlayerObjScript = currPlayerObj.GetComponent<ControllableObject>();
        currPlayerObjCam = currPlayerObj.GetComponent<ControllableObject>().objCamera;

        currPlayerObjCam.enabled = true;

        // Sets online animation on switched in minibot
        if (currPlayerObjScript.thisElectronicType == ElectronicType.Humanoid)
            currPlayerObjScript.animator.SetBool("isOnline", true);

        // Assigns correct y axis value for different electronic types
        if (currPlayerObjScript.thisElectronicType == ElectronicType.Camera)
        {
            currPlayerObjCam.m_YAxis.Value = 0.55f;
            currPlayerObjCam.m_XAxis.Value = currPlayerObj.GetComponent<ObserverCamFix>().xValueModifier + 90;
        }
        else
            currPlayerObjCam.m_YAxis.Value = 0.55f;

        // Debugging
        Debug.Log("Currently controlling " + currPlayerObj);
    }
}
