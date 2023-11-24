using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPlayer_Turret : AudioPlayerBase
{
    public override void Init()
    {
        base.Init();
        instance = this;
    }

    public static AudioPlayer_Turret instance;
    public enum EAudioType_Turret { NONE = -1, ATTACK, LENGTH } 
}