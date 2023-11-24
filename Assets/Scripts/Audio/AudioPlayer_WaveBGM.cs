using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPlayer_WaveBGM : BGMPlayerBase
{
    public override void Init()
    {
        base.Init();
        instance = this;
    }

    /// <summary>
    /// delay초 이후에 재생
    /// </summary>
    /// <param name="delay"></param>
    public void PlayAudioWithDelay(float delay = 1.0f)
    {
        StartCoroutine(PlayAfterDelay(delay));
    }

    private IEnumerator PlayAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        PlayAudio();
    }
    

    public static AudioPlayer_WaveBGM instance;
    public enum EAudioType_WaveBGM 
    { 
        NONE = -1, 
        WaveBGM_01, 
        WaveBGM_02, 
        WaveBGM_03, 
        LENGTH 
    }
}
