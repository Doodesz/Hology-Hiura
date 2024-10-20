using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelCompleteButtons : MonoBehaviour
{
    public string nextLevelSceneName;

    public void GoToNextLevel()
    {
        SceneManager.LoadSceneAsync(nextLevelSceneName);
        Time.timeScale = 1.0f;
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadSceneAsync("MainMenu");
        Time.timeScale = 1.0f;
    }
}
