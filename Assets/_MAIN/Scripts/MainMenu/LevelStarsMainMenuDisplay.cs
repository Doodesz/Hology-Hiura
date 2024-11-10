using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelStarsMainMenuDisplay : MonoBehaviour
{
    [System.Serializable]
    public struct LevelStars
    {
        public string starAKey;
        public Image starAIcon;
        public string starBKey;
        public Image starBIcon;
        public string starCKey;
        public Image starCIcon;
    }

    [SerializeField] List<LevelStars> levelStarsList;
    [SerializeField] Sprite starCompleted;
    [SerializeField] Sprite starIncomplete;

    // Start is called before the first frame update
    void Start()
    {
        LoadStars();
    }

    public void LoadStars()
    {
        // For each levels
        foreach (var levelStar in levelStarsList)
        {
            // For A stars
            if (PlayerPrefs.GetInt(levelStar.starAKey, 0) == 0)
                levelStar.starAIcon.sprite = starIncomplete;
            else
                levelStar.starAIcon.sprite = starCompleted;

            // For B stars
            if (PlayerPrefs.GetInt(levelStar.starBKey, 0) == 0)
                levelStar.starBIcon.sprite = starIncomplete;
            else
                levelStar.starBIcon.sprite = starCompleted;

            // For C stars
            if (PlayerPrefs.GetInt(levelStar.starCKey, 0) == 0)
                levelStar.starCIcon.sprite = starIncomplete;
            else
                levelStar.starCIcon.sprite = starCompleted;
        }
    }
}
