using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class AudioPlayer_Enemy : AudioPlayerBase
{
    public override void Init()
    {
        base.Init();
        instance = this;
    }

    public static AudioPlayer_Enemy instance;

    public enum EAudioType_Enemy { NONE = -1, SELECT_01, SELECT_02, SELECT_03, ATTACK, DEAD, LENGTH } 
}
