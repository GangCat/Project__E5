using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity;

public class BGMPlayerBase : AudioPlayerBase
{
    public void PlayAudio()
    {
        // 0부터 LENGTH-1까지 랜덤 선택
        int newClipIdx = Random.Range(0, (int)EAudioType_BGM.LENGTH);

        audioSource.clip = myAudioClips[newClipIdx];
        audioSource.Play();

        StartCoroutine(PlayNextWhenFinished(audioSource));
    }

    protected IEnumerator PlayNextWhenFinished(AudioSource source)
    {
        // 현재 오디오 클립의 길이만큼 기다립니다.
        yield return new WaitForSeconds(source.clip.length);

        // 현재 오디오가 끝나면 다음 오디오를 재생합니다.
        PlayAudio();
    }

    public void StopAudio()
    {
        if (audioSource.isPlaying)
            audioSource.Stop();

        StopAllCoroutines();
    }

    public void StopAudioWithFade(float fadeDuration = 1.0f)
    {
        StartCoroutine(FadeOut(fadeDuration));
    }

    protected IEnumerator FadeOut(float duration)
    {
        float startVolume = audioSource.volume;

        while (audioSource.volume >= 0)
        {
            audioSource.volume -= startVolume * Time.deltaTime / duration;
            yield return null;
        }

        audioSource.Stop();
        audioSource.volume = startVolume;

        // 진행 중이던 Coroutine을 중단합니다.
        StopAllCoroutines();
    }

}
