using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AudioPlayerBase : MonoBehaviour
{
    public enum EAudioPlayerType{NONE = -1, BGM, EFFECT }
    public abstract void Init();

    public EAudioPlayerType AudioType => audioType;
    
    // audioPlayers �ʵ带 protected�� �����Ͽ� �Ļ� Ŭ�������� ������ �� �ְ� �մϴ�.
    protected AudioSource[] audioPlayersbase;

    public virtual void SetVolume(float _volume)
    {
        audioPlayerVolume = _volume;
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
}
