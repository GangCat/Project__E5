using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPlayer_Struct : AudioPlayerBase
{
    public override void Init()
    {
        base.Init();
        instance = this;
    }
    
    public static AudioPlayer_Struct instance;
    public enum EAudioType_Struct { NONE = -1, BUILD, DESTROY, MAIN_BASE, NUCLEAR, TURRET, BARRACK, BUNKER, WALL,  LENGTH } 
}
