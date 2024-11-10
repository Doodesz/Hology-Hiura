using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class Objective
{
    public bool isCompleted;
    public string desc;
    public ObjectiveObject objectiveObject;
}

public class ObjectiveManager : MonoBehaviour
{
    public delegate void ObjectiveDelegate();
    public static event ObjectiveDelegate OnLevelComplete;

    [Header("Variables")]
    public List<Objective> objectives;
    [SerializeField] string levelKey;
    [SerializeField] string nextLevelKey;

    [Header("Debugging")]
    [SerializeField] TextMeshProUGUI objectiveText;
    public int currIndex;

    [HideInInspector] public static ObjectiveManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        // Get ref
        objectiveText = MainIngameUI.Instance.objectiveText;

        // Sets curr objective to 0
        currIndex = 0;
        foreach (var objective in objectives)
        {
            objective.isCompleted = false;
        }
        objectiveText.text = objectives[currIndex].desc;
    }

    public void UpdateNewObjective()
    {
        // Resets before checking for new value
        currIndex = 0;

        // Updates objective list based on their object completion state
        foreach (var objective in objectives)
        { 
            objective.isCompleted = objective.objectiveObject.isCompleted;
        }

        // Updates currIndex based on the first incomplete objective object in list
        foreach (var objective in objectives)
        {
            if (!objective.isCompleted)
                break;
            currIndex++;
        }

        

        // If next index is beyond available index, complete level
        if (currIndex >= objectives.Count)
        {
            CompleteLevel();
        }
        else
        {
            // Update instruction and indicator
            objectiveText.text = objectives[currIndex].desc;

            foreach (var objective in objectives)
                objective.objectiveObject.objectiveIcon.enabled = false;

            objectives[currIndex].objectiveObject.objectiveIcon.enabled = true;
        }
    }

    private void CompleteLevel()
    {
        OnLevelComplete();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 0f;

        foreach (AudioSource audio in FindObjectsOfType(typeof(AudioSource)))
        {
            if (audio.name != "On Complete Sfx")
                audio.volume = 0.05f;
        }

        BlurManager.Instance.BlurCamera();
        GameManager.Instance.MuteAllSceneSfx();
        GameManager.Instance.hasCompletedLevel = true;
        StarsManager.Instance.CompleteLevelCompleteStars();
        StarsManager.Instance.CompleteNoDisabledBotStars();
        StarsManager.Instance.CompleteTimeTrialStars();
        SyncStarsIconNText.Instance.Sync();

        //Debug.Log("Level completed!");

        SaveLevel();
    }

    void SaveLevel()
    {
        PlayerPrefs.SetInt(levelKey, 1);
        if (nextLevelKey != "-" || nextLevelKey != string.Empty)
            PlayerPrefs.SetInt(nextLevelKey, 1);
        PlayerPrefs.Save();
        PlayerPrefs.SetString("lastPlayedLevel", MainIngameUI.Instance.GetComponent<LevelCompleteButtons>().nextLevelSceneName);
    }
}
