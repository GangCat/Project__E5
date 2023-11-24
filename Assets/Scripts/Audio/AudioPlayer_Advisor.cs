using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.HighDefinition.CameraSettings;

public class AudioPlayer_Advisor : AudioPlayerBase
{
    public override void Init()
    {
        base.Init();
        instance = this;
    }

    public static AudioPlayer_Advisor instance;
    public enum EAudioType_Advisor 
    { 
        NONE = -1, 
        ENERGY, 
        CORE, 
        RESEARCH, 
        UPGRADE, 
        CONST_COMPLETE,
        CONST_CANCEL, 
        PAUSE, 
        RESUME, 
        NUCLEAR_READY, 
        NUCLEAR_LAUNCH, 
        UNDERATTACK_01, 
        UNDERATTACK_02, 
        UNDERATTACK_03, 
        LENGTH 
    } 
}
