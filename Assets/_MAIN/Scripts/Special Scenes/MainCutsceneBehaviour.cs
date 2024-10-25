using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

public class MainCutsceneBehaviour : MonoBehaviour
{
    [SerializeField] VideoPlayer vid;
    [SerializeField] Image skipIcon;
    [SerializeField] string targetScene;
    [SerializeField] float skipValue;

    void Start()
    {
        vid.loopPointReached += CheckOver;

        vid = GetComponent<VideoPlayer>();
    }

    private void Update()
    {
        if (Input.anyKey)
        {
            skipValue += Time.unscaledDeltaTime;
            skipIcon.fillAmount = skipValue / 3f;
        }
        else
        {
            skipValue = 0;
            skipIcon.fillAmount = 0f;
        }
        
        if (skipValue > 3f)
            CheckOver(vid);
    }

    void CheckOver(VideoPlayer vp)
    {
        SceneManager.LoadScene(targetScene);
    }
}
