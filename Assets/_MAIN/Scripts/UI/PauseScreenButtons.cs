using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseScreenButtons : MonoBehaviour
{
    public void OnResumeClicked()
    {
        MainIngameUI.Instance.buttonAudio.Play();
        GameManager.Instance.UnpauseGame();
    }

    public void OnRestartClicked()
    {
        MainIngameUI.Instance.buttonAudio.Play();
        GameManager.Instance.GoToScene(SceneManager.GetActiveScene().name);
    }

    public void OnQuitClicked()
    {
        MainIngameUI.Instance.exitButtonAudio.Play();
        GameManager.Instance.QuitToMainMenu();
    }
}
