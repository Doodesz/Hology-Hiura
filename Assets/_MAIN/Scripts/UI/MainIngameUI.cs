using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MainIngameUI : MonoBehaviour
{
    [Header("References")]
    public Animator anim;
    public TextMeshProUGUI objectiveText;
    public GameObject levelCompleteObj;
    public GameObject pauseScreen;
    public List<GameObject> hiddenObjsOnPause = new List<GameObject>();

    [Header("Audio References")]
    public AudioSource buttonAudio;
    public AudioSource exitButtonAudio;

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
        levelCompleteObj.SetActive(true);
    }
}
