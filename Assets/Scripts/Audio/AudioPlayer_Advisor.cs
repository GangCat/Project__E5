using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPlayer_Advisor : AudioPlayerBase
{
    public override void Init()
    {
        instance = this;

        audioPlayers = new AudioSource();
        AudioManager.AudioVolumes volumes = AudioManager.instance.Volumes;

        audioPlayers = gameObject.AddComponent<AudioSource>();
        audioPlayers.playOnAwake = false;
        audioPlayers.volume = volumes.Effect;
    }

    public override void SetVolume(float _volume)
    {
        base.SetVolume(_volume);
        audioPlayers.volume = _volume;
    }

    public void PlayAudio(EAudioType_Advisor _audioType)
    {
        audioPlayers.clip = audioClips[(int)_audioType];
        audioPlayers.Play();
    }
    
    [Header("#AdvisorAudio")]
    [SerializeField] private AudioClip[] audioClips;
    
    private AudioSource audioPlayers;

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
