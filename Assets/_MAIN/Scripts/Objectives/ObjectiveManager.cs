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
    //public delegate void ObjectiveDelegate();
    //public static event ObjectiveDelegate OnObjectiveComplete;

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
        // Get next objective
        objectives[currIndex].isCompleted = true;
        currIndex++;

        // Update instruction
        objectiveText.text = objectives[currIndex].desc;
    }

    
}
