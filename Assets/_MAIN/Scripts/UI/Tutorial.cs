using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Tutorial : MonoBehaviour
{
    [Header("Variables")]
    [SerializeField] bool hasTutorial;

    [Header("References")]
    [SerializeField] GameObject tutorial;
    [SerializeField] Animator anim;
    [SerializeField] Scrollbar scrollbar;

    public static Tutorial Instance;

    // Start is called before the first frame update
    void Start()
    {
        if (hasTutorial)
            StartCoroutine(DelayTutorial());
        else
            tutorial.SetActive(false);
    }

    private void Update()
    {
        if (anim.GetCurrentAnimatorStateInfo(3).IsName("Trigger State Exit") && GameManager.Instance.isTutorial)
        {
            HideTutorial();
        }
    }

    public void CloseTutorial()
    {
        anim.SetTrigger("toggleTutorial");
    }

    void ShowTutorial()
    {
        tutorial.SetActive(true);

        anim.SetTrigger("toggleTutorial");

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        PlayerController.Instance.currPlayerObj.GetComponent<ControllableElectronic>().objCamera.enabled = false;

        Time.timeScale = 0f;
        GameManager.Instance.HideUIObjects();
        GameManager.Instance.MuteAllSceneSfx();
        GameManager.Instance.isTutorial = true;
        BlurManager.Instance.BlurCamera();

        scrollbar.value = 0f;
    }

    public void HideTutorial()
    {
        tutorial.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        PlayerController.Instance.currPlayerObj.GetComponent<ControllableElectronic>().objCamera.enabled = true;

        Time.timeScale = 1f;
        GameManager.Instance.ShowUIObjects();
        GameManager.Instance.UnmuteAllSceneSfx();
        GameManager.Instance.isTutorial = false;
        BlurManager.Instance.UnblurCamera();
    }
    
    IEnumerator DelayTutorial()
    {
        yield return new WaitForNextFrameUnit();

        ShowTutorial();
    }
}
