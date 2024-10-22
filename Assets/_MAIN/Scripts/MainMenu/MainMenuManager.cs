using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum ExitingTo { None, EnterLevel, Quit };

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] Animator anim;
    [SerializeField] bool isExitingScene;
    [SerializeField] ExitingTo exitingTo;
    public string targetScene;

    public static MainMenuManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    public void OnPlayClick()
    {
        if (!isExitingScene)
        {
            anim.SetTrigger("exitScene");
            isExitingScene = true;
            exitingTo = ExitingTo.EnterLevel;

            // load last saved level

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
        if (!isExitingScene)
        {
            anim.SetTrigger("exitScene");
            isExitingScene = true;
            exitingTo = ExitingTo.Quit;

        }
    }
}
