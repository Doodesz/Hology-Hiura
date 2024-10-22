using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public bool gamePaused;

    [Header("Debugging")]
    [SerializeField] MainIngameUI ui;
    [SerializeField] GameObject pauseScreen;
    List<Canvas> chaosbotCanvases = new List<Canvas>();
    [SerializeField] List<GameObject> hiddenObjsOnPause;
    [SerializeField] bool exitingScene;

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
        hiddenObjsOnPause = ui.hiddenObjsOnPause;

        Time.timeScale = 1f;

        foreach (Canvas canvas in FindObjectsOfType(typeof(Canvas)))
        {
            if (canvas.transform.parent.gameObject.layer == 12)
                chaosbotCanvases.Add(canvas);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !exitingScene)
        {
            OnPauseClicked();
        }

        if (ui.anim.GetCurrentAnimatorStateInfo(2).IsName("Trigger State Quit"))
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene("MainMenu");
        }
    }

    public void TogglePauseGame()
    {
        if (!exitingScene)
        {
            if (!gamePaused && (ui.anim.GetCurrentAnimatorStateInfo(1).IsName("Unpaused")
                || ui.anim.GetCurrentAnimatorStateInfo(1).IsName("New State")))
                PauseGame();
            else if (gamePaused)
                UnpauseGame();
        }
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

        foreach (Canvas canvas in chaosbotCanvases)
            canvas.gameObject.SetActive(false);

        foreach (GameObject obj in hiddenObjsOnPause)
            obj.SetActive(false);
    }

    public void UnpauseGame()
    {
        gamePaused = false;

        Time.timeScale = 1f;

        ui.anim.SetBool("isPaused", false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        PlayerController.Instance.currPlayerObj.GetComponent<ControllableElectronic>().objCamera.enabled = true;

        BlurManager.Instance.UnblurCamera();

        foreach (Canvas canvas in chaosbotCanvases)
            canvas.gameObject.SetActive(true);

        foreach (GameObject obj in hiddenObjsOnPause)
            obj.SetActive(true);
    }

    public void QuitToMainMenu()
    {
        ui.anim.SetTrigger("exitScene");
        exitingScene = true;
    }
}
