using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

enum ExitingTo { None, EnterLevel, Quit };

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] Animator anim;
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

            GoToScene("Level 1");
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
}
