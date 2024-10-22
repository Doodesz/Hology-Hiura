using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseScreenButtons : MonoBehaviour
{
    public void OnResumeClicked()
    {
        GameManager.Instance.UnpauseGame();
    }

    public void OnQuitClicked()
    {
        GameManager.Instance.QuitToMainMenu();
    }
}
