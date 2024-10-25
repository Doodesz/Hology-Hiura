using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseScreenButtons : MonoBehaviour
{
    public void OnResumeClicked()
    {
        GameManager.Instance.UnpauseGame();
    }

    public void OnRestartClicked()
    {
        GameManager.Instance.GoToScene(SceneManager.GetActiveScene().name);
    }

    public void OnQuitClicked()
    {
        GameManager.Instance.QuitToMainMenu();
    }
}
