using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ObjectiveManager : MonoBehaviour
{
    struct Objective
    {
        public bool isCompleted;
        public string desc;

        public void SetComplete() { isCompleted = true; }
        public void SetIncomplete() { isCompleted = false; }
    }

    //public delegate void ObjectiveDelegate();
    //public static event ObjectiveDelegate OnObjectiveComplete;


    [Header("References")]
    [SerializeField] TextMeshProUGUI objectiveText;

    [Header("Variables")]
    List<Objective> objectives;
    public int currIndex;

    [HideInInspector] public static ObjectiveManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    public void UpdateNewObjective()
    {
        // Get next objective
        objectives[currIndex].SetComplete();
        currIndex++;

        // Update instruction
        objectiveText.text = objectives[currIndex].desc;
    }

    
}
