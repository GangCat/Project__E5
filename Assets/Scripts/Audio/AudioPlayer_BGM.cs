using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPlayer_BGM : BGMPlayerBase
{
    public override void Init()
    {
        base.Init();
        instance = this;
    }


    public void PlayAudio_MainMenu(EAudioType_BGM _audioType = EAudioType_BGM.BGM_MAINMENU)
    {
        audioSource.clip = myAudioClips[(int)_audioType];
        audioSource.Play();
    }


    public static AudioPlayer_BGM instance;
    public enum EAudioType_BGM { NONE = -1, BGM_01, BGM_02, BGM_03, BGM_04, BGM_05, BGM_MAINMENU, LENGTH }
}
