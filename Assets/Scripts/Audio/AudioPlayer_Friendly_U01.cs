using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPlayer_Friendly_U01 : AudioPlayerBase
{
    public override void Init()
    {
        base.Init();
        instance = this;
    }

    public static AudioPlayer_Friendly_U01 instance;

    public enum EAudioType_Friendly_U01
    {
        NONE = -1,
        PRODUCE,
        SELECT_01,
        SELECT_02,
        SELECT_03,
        ORDER_01,
        ORDER_02,
        ORDER_03,
        ORDER_04,
        ATTACK,
        DEAD,
        LENGTH
    }
}
