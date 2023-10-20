using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPlayer_BGM : AudioPlayerBase
{
    public override void Init()
    {
        instance = this;
        audioPlayers = new AudioSource[audioChannels];
        AudioManager.AudioVolumes volumes = AudioManager.instance.Volumes;

        for (int i = 0; i < audioPlayers.Length; ++i)
        {
            audioPlayers[i] = this.gameObject.AddComponent<AudioSource>();
            audioPlayers[i].playOnAwake = false;
            audioPlayers[i].volume = volumes.BGM;
            audioPlayers[i].loop = false;
        }
    }
    
    public void PlayAudio(EAudioType_BGM _audioType = EAudioType_BGM.NONE)
    {
        if(_audioType == EAudioType_BGM.NONE)
            _audioType = (EAudioType_BGM)UnityEngine.Random.Range(1, (int)EAudioType_BGM.LENGTH); // 1���� LENGTH-1���� ���� ����
        
        for (int i = 0; i < audioPlayers.Length; ++i)
        {
            int loopIndex = (i + channelIndex) % audioPlayers.Length;

            channelIndex = loopIndex;
            audioPlayers[loopIndex].clip = audioClips[(int)_audioType];
            audioPlayers[loopIndex].Play();

            StartCoroutine(PlayNextWhenFinished(audioPlayers[loopIndex]));
            break;
        }
    }
    
    private IEnumerator PlayNextWhenFinished(AudioSource source)
    {
        // ���� ����� Ŭ���� ���̸�ŭ ��ٸ��ϴ�.
        yield return new WaitForSeconds(source.clip.length);
        
        // ���� ������� ������ ���� ������� ����մϴ�.
        PlayAudio();
    }

    public void StopAudio()
    {
        foreach (var audioPlayer in audioPlayers)
        {
            if (audioPlayer.isPlaying)
            {
                audioPlayer.Stop();
            }
        }
        
        StopAllCoroutines();
    }

    public void StopAudioWithFade(float fadeDuration = 1.0f)
    {
        StartCoroutine(FadeOut(fadeDuration));
    }

    private IEnumerator FadeOut(float duration)
    {
        float startVolume = audioPlayers[channelIndex].volume;

        while (audioPlayers[channelIndex].volume > 0)
        {
            audioPlayers[channelIndex].volume -= startVolume * Time.deltaTime / duration;
            yield return null;
        }

        audioPlayers[channelIndex].Stop();
        audioPlayers[channelIndex].volume = startVolume;

        // ���� ���̴� Coroutine�� �ߴ��մϴ�.
        StopAllCoroutines();
    }
    
    
    [Header("#BGM_Audio")]
    [SerializeField] private AudioClip[] audioClips;
    // [SerializeField] private float audioVolume;
    [SerializeField] private int audioChannels; 
    private AudioSource[] audioPlayers;

    private int channelIndex;
    
    public static AudioPlayer_BGM instance;
    public enum EAudioType_BGM { NONE = -1, BGM_01, BGM_02, BGM_03, BGM_04, BGM_05, LENGTH }
}
