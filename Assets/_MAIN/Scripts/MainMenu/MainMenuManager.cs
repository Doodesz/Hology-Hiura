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

    [Header("Debugging")]
    [SerializeField] bool exitingScene;
    [SerializeField] ExitingTo exitingTo;
    public string newSceneName;

    public static MainMenuManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (anim.GetCurrentAnimatorStateInfo(1).IsName("Trigger State"))
        {
            SceneManager.LoadScene(newSceneName);
        }
    }

    public void OnPlayClick()
    {
        if (!exitingScene)
        {
            /*anim.SetTrigger("exitScene");
            exitingScene = true;
            exitingTo = ExitingTo.EnterLevel;*/

            // load last saved level

            GoToScene(newGameSceneName);
        }
    }

    public void OnLevelSelectClick()
    {
        anim.SetTrigger("selectLevelClick");
    }

    public void OnBackClick()
    {
        anim.SetTrigger("back");
    }

    public void OnExitClick()
    {
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
        GoToScene("Level 1.1");
    }

    public void OnLevel1_2Clicked()
    {
        GoToScene("Level 1.2");
    }

    public void OnLevel2_1Clicked()
    {
        GoToScene("Level 2.1");
    }

    public void OnLevel2_2Clicked()
    {
        GoToScene("Level 2.2");
    }

    public void OnLevel3_1Clicked()
    {
        GoToScene("Level 3.1");
    }

    public void OnLevel3_2Clicked()
    {
        GoToScene("Level 3.2");
    }

    public void OnLevel4_1Clicked()
    {
        GoToScene("Level 4.1");
    }

    public void OnLevel4_2Cliced()
    {
        GoToScene("Level 4.2");
    }
}
