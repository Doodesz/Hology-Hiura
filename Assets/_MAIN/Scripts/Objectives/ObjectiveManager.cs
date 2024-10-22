using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

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
            audio.volume = 0.05f;

        BlurManager.Instance.BlurCamera();

        Debug.Log("Level completed!");
    }
}
