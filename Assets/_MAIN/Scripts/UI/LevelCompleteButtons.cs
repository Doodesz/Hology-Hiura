using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelCompleteButtons : MonoBehaviour
{
    public string nextLevelSceneName;

    public void GoToNextLevel()
    {
        GameManager.Instance.GoToScene(nextLevelSceneName);
    }

    public void GoToMainMenu()
    {
        GameManager.Instance.QuitToMainMenu();
    }
}
