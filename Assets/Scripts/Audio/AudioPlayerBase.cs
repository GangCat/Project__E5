using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AudioPlayerBase : MonoBehaviour
{
    public abstract void Init();
    
    // audioPlayers �ʵ带 protected�� �����Ͽ� �Ļ� Ŭ�������� ������ �� �ְ� �մϴ�.
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
    
    // ���� ������� ��� ������ Ȯ���ϴ� �޼��带 �߰��մϴ�.
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
