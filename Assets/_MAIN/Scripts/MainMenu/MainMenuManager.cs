using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

enum ExitingTo { None, EnterLevel, Quit };

public class MainMenuManager : MonoBehaviour
{
    [Header("Variables")]
    [SerializeField] string newGameSceneName;
    [SerializeField] Animator anim;
    [SerializeField] GameObject continueButton;
    [SerializeField] GameObject continueIcon;

    [Header("Audio References")]
    [SerializeField] AudioSource clickSfx;
    [SerializeField] AudioSource backSfx;

    [Header("Debugging")]
    [SerializeField] bool exitingScene;
    [SerializeField] ExitingTo exitingTo;
    public string newSceneName;
    [SerializeField] float specialKeyHoldValue;

    public static MainMenuManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (!PlayerPrefs.HasKey("lastPlayedLevel") || PlayerPrefs.GetString("lastPlayedLevel") == "Epilogue"
            || PlayerPrefs.GetString("lastPlayedLevel") == "MainMenu")
        {
            continueButton.SetActive(false);
            continueIcon.SetActive(false);
        }
    }

    private void Update()
    {
        if (anim.GetCurrentAnimatorStateInfo(1).IsName("Trigger State"))
        {
            SceneManager.LoadScene(newSceneName);
        }

        if (Input.GetKey(KeyCode.H) && Input.GetKey(KeyCode.I))
        {
            specialKeyHoldValue += Time.deltaTime;

            if (specialKeyHoldValue > 3)
            {
                LevelLockManager.Instance.UnlockAllLevels();
            }
        }
        else if (Input.GetKey(KeyCode.U) && Input.GetKey(KeyCode.R) && Input.GetKey(KeyCode.A))
        {
            specialKeyHoldValue += Time.deltaTime;

            if (specialKeyHoldValue > 3)
            {
                LevelLockManager.Instance.DeletePlayerPrefs();
            }
        }
        else
            specialKeyHoldValue = 0;
    }

    public void OnContinueClick()
    {
        clickSfx.Play();
        GoToScene(PlayerPrefs.GetString("lastPlayedLevel", "Prologue"));
    }

    public void OnPlayClick()
    {
        clickSfx.Play();
        if (!exitingScene)
        {
            GoToScene(newGameSceneName);
        }
    }

    public void OnLevelSelectClick()
    {
        clickSfx.Play();
        anim.SetTrigger("selectLevelClick");
    }

    public void OnBackClick()
    {
        backSfx.Play();
        anim.SetTrigger("back");
    }

    public void OnExitClick()
    {
        backSfx.Play();
        if (!exitingScene)
        {
            anim.SetTrigger("exitScene");
            exitingScene = true;
            exitingTo = ExitingTo.Quit;

            Application.Quit();
        }
    }

    public void GoToScene(string sceneName)
    {
        if (!exitingScene)
        {
            anim.SetTrigger("exitScene");
            exitingScene = true;
            newSceneName = sceneName;
        }
    }

    public void OnLevel1_1Clicked()
    {
        clickSfx.Play();
        GoToScene("Level 1.1");
    }

    public void OnLevel1_2Clicked()
    {
        clickSfx.Play();
        GoToScene("Level 1.2");
    }

    public void OnLevel2_1Clicked()
    {
        clickSfx.Play();
        GoToScene("Level 2.1");
    }

    public void OnLevel2_2Clicked()
    {
        clickSfx.Play();
        GoToScene("Level 2.2");
    }

    public void OnLevel3_1Clicked()
    {
        clickSfx.Play();
        GoToScene("Level 3.1");
    }

    public void OnLevel3_2Clicked()
    {
        clickSfx.Play();
        GoToScene("Level 3.2");
    }

    public void OnLevel4Clicked()
    {
        clickSfx.Play();
        GoToScene("Level 4");
    }

    public void OnLevel4_1Clicked()
    {
        clickSfx.Play();
        GoToScene("Level 4.1");
    }

    public void OnLevel4_2Cliced()
    {
        clickSfx.Play();
        GoToScene("Level 4.2");
    }

    public void OnHiddenLevelClicked()
    {
        clickSfx.Play();
        GoToScene("Level Hidden");
    }

    public void OnLevelButtonClicked(string levelSceneName)
    {
        clickSfx.Play();
        GoToScene(levelSceneName);
    }

    void UnlockAllLevels()
    {

    }
}
