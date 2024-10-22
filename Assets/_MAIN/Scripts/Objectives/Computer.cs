using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class Computer : MonoBehaviour
{
    [Header("References")]
    public VideoPlayer vid;
    public ObjectiveObject objective;
    [SerializeField] Animator anim;

    void Start() 
    { 
        vid.loopPointReached += CheckOver;

        vid = GetComponent<VideoPlayer>();
    }

    void CheckOver(VideoPlayer vp)
    {
        Time.timeScale = 1f;

        foreach (AudioSource audio in FindObjectsOfType(typeof(AudioSource)))
        {
            audio.mute = false;
        }
        
        vid.Stop();

        anim.SetBool("showVideo", false);

        GameManager.Instance.ShowUIObjects();
        GameManager.Instance.isPlayingAVideo = false;
    }

    public void PlayVideo()
    {
        if (vid.clip == null)
            return;

        Time.timeScale = 0f;

        foreach (AudioSource audio in FindObjectsOfType(typeof(AudioSource)))
        {
            audio.mute = true;
        }

        vid.Play();

        anim.SetBool("showVideo", true);

        GameManager.Instance.HideUIObjects();
        GameManager.Instance.isPlayingAVideo = true;
    }
}
