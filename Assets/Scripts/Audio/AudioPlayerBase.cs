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
    
    // audioPlayers �ʵ带 protected�� �����Ͽ� �Ļ� Ŭ�������� ������ �� �ְ� �մϴ�.
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

    // ���� ������� ��� ������ Ȯ���ϴ� �޼��带 �߰��մϴ�.
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
    // ������� ����� ������ҽ�
    protected AudioSource audioSource;
    // ����� ����� Ŭ���� �ε���
    protected int audioIdx = 0;
}
