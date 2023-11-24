using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPlayer_Misc : AudioPlayerBase
{
    public override void Init()
    {
        base.Init();
        instance = this;
    }

    
    public static AudioPlayer_Misc instance;
    public enum EAudioType_Misc { NONE = -1, NUCLEAR_EXPLOSION, PICKUP, LENGTH } 
}
