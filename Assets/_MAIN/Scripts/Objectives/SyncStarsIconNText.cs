using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SyncStarsIconNText : MonoBehaviour
{
    [Header("References")]
    public TextMeshProUGUI starAText;
    public TextMeshProUGUI starBText;
    public TextMeshProUGUI starCText;
    public Image starAIcon;
    public Image starBIcon;
    public Image starCIcon;

    [Header("Reference to copy")]
    public Image starAIconRef;
    public Image starBIconRef;
    public Image starCIconRef;

    StarsManager starsManager;
    public static SyncStarsIconNText Instance;

    private void Start()
    {
        starsManager = StarsManager.Instance;
        Instance = this;
    }

    public void Sync()
    {
        starAIcon.sprite = StarsManager.Instance.starAIcon.sprite;
        starAText.text = StarsManager.Instance.starAText.text;
        starBIcon.sprite = StarsManager.Instance.starBIcon.sprite;
        starBText.text = StarsManager.Instance.starBText.text;
        starCIcon.sprite = StarsManager.Instance.starCIcon.sprite;
        starCText.text = StarsManager.Instance.starCText.text;
    }
}
