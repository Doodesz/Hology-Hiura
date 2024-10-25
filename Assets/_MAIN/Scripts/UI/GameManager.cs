using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public bool gamePaused;
    public List<GameObject> hiddenObjsOnPause;

    [Header("Debugging")]
    [SerializeField] MainIngameUI ui;
    [SerializeField] GameObject pauseScreen;
    List<Canvas> chaosbotCanvases = new List<Canvas>();
    [SerializeField] bool exitingScene;
    [SerializeField] bool goingToNewScene;
    [SerializeField] string newSceneName;
    [SerializeField] bool goingToMainMenu;
    public bool isPlayingAVideo;
    public bool isReading;

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

        GetComponent<NavMeshSurface>().BuildNavMesh();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !exitingScene && !isPlayingAVideo && !isReading)
        {
            OnPauseClicked();
        }

        if (ui.anim.GetCurrentAnimatorStateInfo(2).IsName("Trigger State Quit"))
        {
            Time.timeScale = 1f;

            if (goingToMainMenu)
            {
                SceneManager.LoadScene("MainMenu");
            }
            else if (goingToNewScene)
            {
                SceneManager.LoadScene(newSceneName);
            }
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

        HideUIObjects();
        MuteAllSceneSfx();
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

        ShowUIObjects();
        UnmuteAllSceneSfx();
    }

    public void QuitToMainMenu()
    {
        if (!goingToNewScene)
        {
            ui.anim.SetTrigger("exitScene");
            exitingScene = true;
            goingToMainMenu = true;
        }
    }

    public void GoToScene(string sceneName)
    {
        if (!goingToMainMenu)
        {
            ui.anim.SetTrigger("exitScene");
            exitingScene = true;
            goingToNewScene = true;
            newSceneName = sceneName;
        }
    }

    public void HideUIObjects()
    {
        foreach (GameObject obj in hiddenObjsOnPause)
            obj.SetActive(false);
        foreach (Canvas canvas in chaosbotCanvases)
            canvas.gameObject.SetActive(false);
    }

    public void ShowUIObjects()
    {
        foreach (GameObject obj in hiddenObjsOnPause)
            obj.SetActive(true);
        foreach (Canvas canvas in chaosbotCanvases)
            canvas.gameObject.SetActive(true);
    }

    public void MuteAllSceneSfx()
    {
        foreach (ObjectSoundManager sound in FindObjectsOfType<ObjectSoundManager>())
        {
            if (sound.TryGetComponent<ChaosBot>(out ChaosBot chaosBot))
                sound.MuteAll();
            if (sound.TryGetComponent<PlayerMovement>(out PlayerMovement playerMovement))
                sound.MuteAll();
        }
    }
    public void UnmuteAllSceneSfx()
    {
        foreach (ObjectSoundManager sound in FindObjectsOfType<ObjectSoundManager>())
        {
            if (sound.TryGetComponent<ChaosBot>(out ChaosBot chaosBot))
                sound.UnmuteAll();
            if (sound.TryGetComponent<PlayerMovement>(out PlayerMovement playerMovement))
                sound.UnmuteAll();
        }
    }
}
