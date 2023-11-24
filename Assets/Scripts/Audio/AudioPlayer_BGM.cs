using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPlayer_BGM : AudioPlayerBase
{
    public override void Init()
    {
        instance = this;
        audioPlayers = new AudioSource();
        AudioManager.AudioVolumes volumes = AudioManager.instance.Volumes;

        audioPlayers = gameObject.AddComponent<AudioSource>();
        audioPlayers.playOnAwake = false;
        audioPlayers.volume = volumes.BGM;
        audioPlayers.loop = false;
    }

    public override void SetVolume(float _volume)
    {
        base.SetVolume(_volume);
        audioPlayers.volume = _volume;
    }

    public void PlayAudio(EAudioType_BGM _audioType = EAudioType_BGM.NONE)
    {
        if (_audioType == EAudioType_BGM.NONE)
            _audioType = (EAudioType_BGM)Random.Range(1, (int)EAudioType_BGM.LENGTH); // 1부터 LENGTH-1까지 랜덤 선택

        audioPlayers.clip = audioClips[(int)_audioType];
        audioPlayers.Play();

        StartCoroutine(PlayNextWhenFinished(audioPlayers));
    }

    public void PlayAudio_MainMenu(EAudioType_BGM _audioType = EAudioType_BGM.BGM_MAINMENU)
    {
        // _audioType = EAudioType_BGM.BGM_MAINMENU;
        audioPlayers.clip = audioClips[(int)_audioType];
        audioPlayers.Play();
    }

    private IEnumerator PlayNextWhenFinished(AudioSource source)
    {
        // 현재 오디오 클립의 길이만큼 기다립니다.
        yield return new WaitForSeconds(source.clip.length);

        // 현재 오디오가 끝나면 다음 오디오를 재생합니다.
        PlayAudio();
    }

    public void StopAudio()
    {
        if (audioPlayers.isPlaying)
        {
            audioPlayers.Stop();
        }

        StopAllCoroutines();
    }

    public void StopAudioWithFade(float fadeDuration = 1.0f)
    {
        StartCoroutine(FadeOut(fadeDuration));
    }

    private IEnumerator FadeOut(float duration)
    {
        float startVolume = audioPlayers.volume;

        while (audioPlayers.volume > 0)
        {
            audioPlayers.volume -= startVolume * Time.deltaTime / duration;
            yield return null;
        }

        audioPlayers.Stop();
        audioPlayers.volume = startVolume;

        // 진행 중이던 Coroutine을 중단합니다.
        StopAllCoroutines();
    }


    [Header("#BGM_Audio")]
    [SerializeField] private AudioClip[] audioClips;
    private AudioSource audioPlayers;

    public static AudioPlayer_BGM instance;
    public enum EAudioType_BGM { NONE = -1, BGM_01, BGM_02, BGM_03, BGM_04, BGM_05, BGM_MAINMENU, LENGTH }
}
