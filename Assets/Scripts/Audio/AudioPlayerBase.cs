using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AudioPlayerBase : MonoBehaviour
{
    public enum EAudioPlayerType {NONE = -1, BGM, EFFECT }
    public virtual void Init()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;

        switch (audioType)
        {
            case EAudioPlayerType.BGM:
                audioSource.volume = AudioManager.instance.Volumes.BGM;
                break;
            case EAudioPlayerType.EFFECT:
                audioSource.volume = AudioManager.instance.Volumes.Effect;
                break;
            default:
                break;
        }
    }

    public EAudioPlayerType AudioType => audioType;
    
    // audioPlayers 필드를 protected로 선언하여 파생 클래스에서 접근할 수 있게 합니다.
    protected AudioSource[] audioPlayersbase;

    public virtual void SetVolume(float _volume)
    {
        audioSource.volume = _volume;
    }

    public virtual void PlayAudio(Enum _audioEnum)
    {
        audioIdx = Convert.ToInt32(_audioEnum);

        if (audioIdx >= 0 && audioIdx < myAudioClips.Length)
        {
            //Debug.Log(_audioEnum.GetType());
            //Debug.Log(audioIdx);
            audioSource.clip = myAudioClips[audioIdx];
            audioSource.Play();
        }
        else
        {
            Debug.LogError("Invalid player audio index");
        }
    }

    // 현재 오디오가 재생 중인지 확인하는 메서드를 추가합니다.
    public bool IsPlaying()
    {
        if (audioPlayersbase == null)
            return false;

        foreach (var audioPlayer in audioPlayersbase)
        {
            if (audioPlayer.isPlaying)
                return true;
        }
        return false;
    }

    protected float audioPlayerVolume;
    
    [SerializeField]
    private EAudioPlayerType audioType = EAudioPlayerType.NONE;
    [SerializeField]
    protected AudioClip[] myAudioClips;
    // 오디오를 재생할 오디오소스
    protected AudioSource audioSource;
    // 재생할 오디오 클립의 인덱스
    protected int audioIdx = 0;
}
