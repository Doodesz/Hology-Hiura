using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public bool gamePaused;

    [Header("Debugging")]
    [SerializeField] MainIngameUI ui;
    [SerializeField] GameObject pauseScreen;

    public static GameManager Instance;

    public delegate void GameManagerDelegate();
    public static event GameManagerDelegate OnPauseClicked;

    private void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        OnPauseClicked += TogglePauseGame;
    }

    private void OnDisable()
    {
        OnPauseClicked -= TogglePauseGame;
    }

    // Start is called before the first frame update
    void Start()
    {
        ui = MainIngameUI.Instance;
        pauseScreen = ui.pauseScreen;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OnPauseClicked();
        }
    }

    public void TogglePauseGame()
    {
        if (!gamePaused && (ui.anim.GetCurrentAnimatorStateInfo(1).IsName("Unpaused")
            || ui.anim.GetCurrentAnimatorStateInfo(1).IsName("New State")))
            PauseGame();
        else if (gamePaused)
            UnpauseGame();
    }

    void PauseGame()
    {
        //pauseScreen.SetActive(true);

        gamePaused = true;

        Time.timeScale = 0f;

        ui.anim.SetBool("showPrompt", false);
        ui.anim.SetBool("isPaused", true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        PlayerController.Instance.currPlayerObj.GetComponent<ControllableElectronic>().objCamera.enabled = false;
        
        BlurManager.Instance.BlurCamera();
    }

    public void UnpauseGame()
    {
        ui.anim.SetBool("isPaused", false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        gamePaused = false;

        Time.timeScale = 1f;

        PlayerController.Instance.currPlayerObj.GetComponent<ControllableElectronic>().objCamera.enabled = true;

        BlurManager.Instance.UnblurCamera();
    }
}
