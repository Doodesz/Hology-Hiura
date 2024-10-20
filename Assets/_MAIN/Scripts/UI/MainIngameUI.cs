using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MainIngameUI : MonoBehaviour
{
    public Animator anim;
    public TextMeshProUGUI objectiveText;

    public static MainIngameUI Instance;

    private void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        ObjectiveManager.OnLevelComplete += ShowLevelCompleteScreen;
    }

    private void OnDisable()
    {
        ObjectiveManager.OnLevelComplete -= ShowLevelCompleteScreen;
    }

    void ShowLevelCompleteScreen()
    {

    }
}
