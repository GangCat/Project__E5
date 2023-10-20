using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AudioPlayerBase : MonoBehaviour
{
    public abstract void Init();
    
    // audioPlayers 필드를 protected로 선언하여 파생 클래스에서 접근할 수 있게 합니다.
    protected AudioSource[] audioPlayers;

    public virtual void SetVolume(float volume)
    {
        if (audioPlayers == null)
            return;

        foreach (var audioPlayer in audioPlayers)
        {
            audioPlayer.volume = volume;
        }
    }
    
    // 현재 오디오가 재생 중인지 확인하는 메서드를 추가합니다.
    public bool IsPlaying()
    {
        if (audioPlayers == null)
            return false;

        foreach (var audioPlayer in audioPlayers)
        {
            if (audioPlayer.isPlaying)
                return true;
        }
        return false;
    }

}
