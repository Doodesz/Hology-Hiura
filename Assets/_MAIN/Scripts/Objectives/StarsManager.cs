using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum StarStatus { Incomplete, Completed, Failed };
public enum StarObjective { CompleteLevel, CompleteInTime, NoBotsDisabled };

[System.Serializable]
public class Star
{
    public string starKeyName;
    public string objectiveInstruction;
    public StarStatus status;       // For checking whether has failed or not 
    public bool isCompleted;        // For checking if it's completed or not on initialization
    public StarObjective objective;

    [Header("Star Modifier")]
    [Tooltip("For CompleteInTime Objective")] public float timeLimit;

    public void CompleteStar()
    {
        status = StarStatus.Completed;
        isCompleted = true;
        PlayerPrefs.SetInt(starKeyName, 1);
    }

    public void LoadStar()
    {
        if (PlayerPrefs.GetInt(starKeyName, 0) == 0)
        {
            status = StarStatus.Incomplete;
            isCompleted = false;
        }
        else
        {
            status = StarStatus.Completed;
            isCompleted = true;
        }
    }

    public void FailStar()
    {
        status = StarStatus.Failed;
        isCompleted = false;
    }
}

public class StarsManager : MonoBehaviour
{
    [Header("Star References")]
    public TextMeshProUGUI starAText;
    public TextMeshProUGUI starBText;
    public TextMeshProUGUI starCText;
    public Image starAIcon;
    public Image starBIcon;
    public Image starCIcon;

    [Header("References")]
    [SerializeField] Sprite incompletedStarIcon;
    [SerializeField] Sprite completedStarIcon;
    [SerializeField] Sprite failedStarIcon;

    [Header("Star")]
    [SerializeField] Star starA;
    [SerializeField] Star starB;
    [SerializeField] Star starC;

    [Header("Debugging")]
    [SerializeField] float timeElapsed;

    public static StarsManager Instance;

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;

        starA.LoadStar();
        starAText.text = starA.objectiveInstruction;
        if (starA.isCompleted)
            starAIcon.sprite = completedStarIcon;

        starB.LoadStar();
        starBText.text = starB.objectiveInstruction;
        if (starB.isCompleted)
            starBIcon.sprite = completedStarIcon;

        starC.LoadStar();
        starCText.text = starC.objectiveInstruction;
        if (starC.isCompleted)
            starCIcon.sprite = completedStarIcon;
    }

    // Update is called once per frame
    void Update()
    {
        timeElapsed += Time.deltaTime;

        if (!starA.isCompleted && starA.objective == StarObjective.CompleteInTime)
        {
            if (timeElapsed < starA.timeLimit)
                starAText.text = starA.objectiveInstruction + " (" + Mathf.Round(timeElapsed) + "/" + starA.timeLimit + " d)";

            // Fail star if timeElapsed exceeded star's timeLimit
            else
            {
                starAText.text = "<s>" + starA.objectiveInstruction + "</s>" + " (" + Mathf.Round(timeElapsed) + "/" + starA.timeLimit + " d)";

                if (starA.status != StarStatus.Failed)
                {
                    starA.FailStar();
                    starAIcon.sprite = failedStarIcon;
                }
            }
        }

        if (!starB.isCompleted && starB.objective == StarObjective.CompleteInTime)
        {
            if (timeElapsed < starB.timeLimit)
                starBText.text = starB.objectiveInstruction + " (" + Mathf.Round(timeElapsed) + "/" + starB.timeLimit + " d)";

            // Fail star if timeElapsed exceeded star's timeLimit
            else
            {
                starBText.text = "<s>" + starB.objectiveInstruction + "</s>" + " (" + Mathf.Round(timeElapsed) + "/" + starB.timeLimit + " d)";

                if (starB.status != StarStatus.Failed)
                {
                    starB.FailStar();
                    starBIcon.sprite = failedStarIcon;
                }
            }
        }

        if (!starC.isCompleted && starC.objective == StarObjective.CompleteInTime)
        {
            if (timeElapsed < starC.timeLimit)
                starCText.text = starC.objectiveInstruction + " (" + Mathf.Round(timeElapsed) + "/" + starC.timeLimit + " d)";

            // Fail star if timeElapsed exceeded star's timeLimit
            else
            {
                starCText.text = "<s>" + starC.objectiveInstruction + "</s>" + " (" + Mathf.Round(timeElapsed) + "/" + starC.timeLimit + " d)";
                
                if (starC.status != StarStatus.Failed)
                {
                    starC.FailStar();
                    starCIcon.sprite = failedStarIcon;
                }
            }
        }
    }

    public void FailNoDisabledBotsStars()
    {
        if (!starA.isCompleted && starA.objective == StarObjective.NoBotsDisabled)
        {
            starA.FailStar();
            starAText.text = "<s>" + starA.objectiveInstruction + "</s>";
            starAIcon.sprite = failedStarIcon;
        }
        if (!starB.isCompleted && starB.objective == StarObjective.NoBotsDisabled)
        {
            starB.FailStar();
            starBText.text = "<s>" + starA.objectiveInstruction + "</s>";
            starBIcon.sprite = failedStarIcon;
        }
        if (!starC.isCompleted && starC.objective == StarObjective.NoBotsDisabled)
        {
            starC.FailStar();
            starCText.text = "<s>" + starC.objectiveInstruction + "</s>";
            starCIcon.sprite = failedStarIcon;
        }
    }

    public void CompleteLevelCompleteStars()
    {
        if (!starA.isCompleted && starA.objective == StarObjective.CompleteLevel)
        {
            starA.CompleteStar();
            starAIcon.sprite = completedStarIcon;
        }
        if (!starB.isCompleted && starB.objective == StarObjective.CompleteLevel)
        {
            starB.CompleteStar();
            starBIcon.sprite = completedStarIcon;
        }
        if (!starC.isCompleted && starC.objective == StarObjective.CompleteLevel)
        {
            starC.CompleteStar();
            starCIcon.sprite = completedStarIcon;
        }
    }

    public void CompleteNoDisabledBotStars()
    {
        if (!starA.isCompleted && starA.objective == StarObjective.NoBotsDisabled && starA.status != StarStatus.Failed)
        {
            starA.CompleteStar();
            starAIcon.sprite = completedStarIcon;
        }
        if (!starB.isCompleted && starB.objective == StarObjective.NoBotsDisabled && starB.status != StarStatus.Failed)
        {
            starB.CompleteStar();
            starBIcon.sprite = completedStarIcon;
        }
        if (!starC.isCompleted && starC.objective == StarObjective.NoBotsDisabled && starC.status != StarStatus.Failed)
        {
            starC.CompleteStar();
            starCIcon.sprite = completedStarIcon;
        }
    }

    public void CompleteTimeTrialStars()
    {
        if (!starA.isCompleted && starA.objective == StarObjective.CompleteInTime && starA.status != StarStatus.Failed)
        {
            starA.CompleteStar();
            starAIcon.sprite = completedStarIcon;
        }
        if (!starB.isCompleted && starB.objective == StarObjective.CompleteInTime && starB.status != StarStatus.Failed)
        {
            starB.CompleteStar();
            starBIcon.sprite = completedStarIcon;
        }
        if (!starC.isCompleted && starC.objective == StarObjective.CompleteInTime && starC.status != StarStatus.Failed)
        {
            starC.CompleteStar();
            starCIcon.sprite = completedStarIcon;
        }
    }
}
