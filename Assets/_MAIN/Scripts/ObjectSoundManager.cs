using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSoundManager : MonoBehaviour
{
    public AudioSource moveSfx;
    public AudioSource disabledUISfx;
    public AudioSource disabledSfx;
    public AudioSource pushSfx;

    public void MuteAll()
    {
        if (moveSfx != null) moveSfx.mute = true;
        if (disabledUISfx != null) disabledUISfx.mute = true;
        if (disabledSfx != null) disabledSfx.mute = true;
        if (pushSfx != null) pushSfx.mute = true;
    }
    public void UnmuteAll()
    {
        if (moveSfx != null) moveSfx.mute = false;
        if (disabledUISfx != null) disabledUISfx.mute = false;
        if (disabledSfx != null) disabledSfx.mute = false;
        if (pushSfx != null) pushSfx.mute = false;
    }

    public void PlayMove()
    {
        moveSfx.Play();
    }
    public void PauseMove()
    {
        moveSfx.Pause();
    }

    public void PlayDisabled()
    {
        disabledSfx.Play();
    }
    public void PauseDisabled()
    {
        disabledSfx.Pause();
    }

    public void PlayDisabledUI()
    {
        disabledUISfx.Play();
    }
    public void PauseDisabledUI()
    {
        disabledUISfx.Pause();
    }

    public void PlayPush()
    {
        pushSfx.Play();
    }
    public void PausePush()
    {
        pushSfx.Pause();
    }
}
