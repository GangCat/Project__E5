using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPlayer_UI : AudioPlayerBase
{
    public override void Init()
    {
        base.Init();
        instance = this;
    }

    public static AudioPlayer_UI instance;
    public enum EAudioType_UI { NONE = -1, CLICK, LENGTH } 
}
