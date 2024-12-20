using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.Video;

public enum PostInteractType { Read, Video };
public class Computer : MonoBehaviour
{
    [Header("References")]
    public VideoPlayer vid;
    public ObjectiveObject objective;
    [SerializeField] Animator anim;

    [SerializeField] GameObject readObj;

    [Header("Variables")]
    public PostInteractType type;

    void Start() 
    {
        if (type == PostInteractType.Video)
        {
            vid.loopPointReached += OnVideoOver;

            vid = GetComponent<VideoPlayer>();
        }
        else
        {

        }
    }

    private void Update()
    {
        if (anim.GetCurrentAnimatorStateInfo(1).IsName("Trigger State Exit") && GameManager.Instance.isReading)
        {
            HideReadable();
        }
    }

    public void OnAfterInteraction()
    {
        if (type == PostInteractType.Video)
            PlayVideo();
        else
            ShowReadable();
    }

    void OnVideoOver(VideoPlayer vp)
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

        PlayerController.Instance.currPlayerObj.GetComponent<ControllableElectronic>().objCamera.enabled = true;
    }

    public void PlayVideo()
    {
        if (vid.clip == null)
        {
            Debug.LogWarning("No video clip assigned!");
            return;
        }

        Time.timeScale = 0f;

        foreach (AudioSource audio in FindObjectsOfType(typeof(AudioSource)))
        {
            audio.mute = true;
        }

        vid.Play();

        anim.SetBool("showVideo", true);

        GameManager.Instance.HideUIObjects();
        GameManager.Instance.isPlayingAVideo = true;

        PlayerController.Instance.currPlayerObj.GetComponent<ControllableElectronic>().objCamera.enabled = false;
    }

    public void CloseReadable()
    {
        anim.SetTrigger("toggleReadable");
        MainIngameUI.Instance.exitButtonAudio.Play();
    }

    void ShowReadable()
    {
        readObj.SetActive(true);

        anim.SetTrigger("toggleReadable");
        MainIngameUI.Instance.buttonAudio.Play();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        PlayerController.Instance.currPlayerObj.GetComponent<ControllableElectronic>().objCamera.enabled = false;

        Time.timeScale = 0f;
        GameManager.Instance.HideUIObjects();
        GameManager.Instance.MuteAllSceneSfx();
        GameManager.Instance.isReading = true;
        BlurManager.Instance.BlurCamera();
    }

    public void HideReadable()
    {
        readObj.SetActive(false);
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        PlayerController.Instance.currPlayerObj.GetComponent<ControllableElectronic>().objCamera.enabled = true;

        Time.timeScale = 1f;
        GameManager.Instance.ShowUIObjects();
        GameManager.Instance.UnmuteAllSceneSfx();
        GameManager.Instance.isReading = false;
        BlurManager.Instance.UnblurCamera();
    }
}
