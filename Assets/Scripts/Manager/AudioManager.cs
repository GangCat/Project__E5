using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public void Init()
    {
        // 싱글톤 패턴 초기화
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        foreach (AudioPlayerBase APB in GetComponentsInChildren<AudioPlayerBase>())
        {
            APB.Init();
            if (APB.AudioType.Equals((AudioPlayerBase.EAudioPlayerType.EFFECT)))
            {
                listEffectPlayer.Add(APB);
            }
            else
            {
                listBGMPlayer.Add(APB);
            }
        }

        SceneManager.sceneLoaded += OnSceneLoaded;
        sliderBGM = 1f;
        sliderEffect = 1f;
    }

    //private void Awake()
    //{
    //    SceneManager.sceneLoaded += OnSceneLoaded;
    //}
    
    
    // Master 볼륨 조절 함수
    public void SetMasterVolume(float _volume)
    {
        audioVolume_Master = _volume * 0.1f;
        audioVolume_BGM = sliderBGM * audioVolume_Master;
        audioVolume_Effect = sliderEffect * audioVolume_Master;

        for(int i =0 ; i < listEffectPlayer.Count; ++i)
            listEffectPlayer[i].SetVolume(audioVolume_Effect);

        for (int i = 0; i < listBGMPlayer.Count; ++i)
        {
            listBGMPlayer[i].SetVolume(audioVolume_BGM);
        }
    }

    // BGM 볼륨 조절 함수
    public void SetBGMVolume(float _volume)
    {
        sliderBGM = _volume * 0.1f;
        audioVolume_BGM = sliderBGM * audioVolume_Master;
        
        for (int i = 0; i < listBGMPlayer.Count; ++i)
            listBGMPlayer[i].SetVolume(audioVolume_BGM);
        
        
    }

    // Effect 볼륨 조절 함수
    public void SetEffectVolume(float _volume)
    {
        sliderEffect = _volume * 0.1f;
        audioVolume_Effect = sliderEffect * audioVolume_Master;
        
        for(int i =0 ; i < listEffectPlayer.Count; ++i)
            listEffectPlayer[i].SetVolume(audioVolume_Effect);
    }
    
    public void PlayAudio_Attack(EObjectType _objectType)
    {
        switch (_objectType)
        {
            case EObjectType.UNIT_01:
                AudioPlayer_Friendly_U01.instance.PlayAudio(AudioPlayer_Friendly_U01.EAudioType_Friendly_U01.ATTACK);
                break;
            case EObjectType.UNIT_02:
                AudioPlayer_Friendly_U02.instance.PlayAudio(AudioPlayer_Friendly_U02.EAudioType_Friendly_U02.ATTACK);
                break;
            case EObjectType.UNIT_HERO:
                AudioPlayer_Hero.instance.PlayAudio(AudioPlayer_Hero.EAudioType_Hero.ATTACK);
                break;
            case EObjectType.ENEMY_UNIT:
                AudioPlayer_Enemy.instance.PlayAudio(AudioPlayer_Enemy.EAudioType_Enemy.ATTACK);
                break;
            case EObjectType.TURRET:
                AudioPlayer_Turret.instance.PlayAudio(AudioPlayer_Turret.EAudioType_Turret.ATTACK);
                break;
            default:
                break;
        }
    }
    
    public void PlayAudio_Order(EObjectType _objectType)
    {
        int Idx = UnityEngine.Random.Range(0, 4); // Generates 0, 1, 2, or 3

        switch (_objectType)
        {
            case EObjectType.UNIT_HERO:
                if (!AudioPlayer_Hero.instance.IsPlaying()) // 오디오가 재생 중이지 않은 경우에만 재생
                {
                    AudioPlayer_Hero.EAudioType_Hero audioTypeHero =
                        (AudioPlayer_Hero.EAudioType_Hero)((int)AudioPlayer_Hero.EAudioType_Hero.ORDER_01 + Idx);
                    AudioPlayer_Hero.instance.PlayAudio(audioTypeHero);
                }
                break;
            case EObjectType.UNIT_01:
                if (!AudioPlayer_Hero.instance.IsPlaying()) // 오디오가 재생 중이지 않은 경우에만 재생
                {
                    AudioPlayer_Friendly_U01.EAudioType_Friendly_U01 audioTypeU01 =
                        (AudioPlayer_Friendly_U01.EAudioType_Friendly_U01)((int)AudioPlayer_Friendly_U01.EAudioType_Friendly_U01.ORDER_01 + Idx);
                    AudioPlayer_Friendly_U01.instance.PlayAudio(audioTypeU01);
                }
                break;
            case EObjectType.UNIT_02:
                if (!AudioPlayer_Hero.instance.IsPlaying()) // 오디오가 재생 중이지 않은 경우에만 재생
                {
                    AudioPlayer_Friendly_U02.EAudioType_Friendly_U02 audioTypeU02 =
                        (AudioPlayer_Friendly_U02.EAudioType_Friendly_U02)((int)AudioPlayer_Friendly_U02.EAudioType_Friendly_U02.ORDER_01 + Idx);
                    AudioPlayer_Friendly_U02.instance.PlayAudio(audioTypeU02);
                }
                break;
            default:
                break;
        }
    }

    public void PlayAudio_Select(EObjectType _objectType)
    {
        int Idx = UnityEngine.Random.Range(0, 3); // Generates 0, 1, 2

        switch (_objectType)
        {
            case EObjectType.UNIT_HERO:
                AudioPlayer_Hero.EAudioType_Hero audioTypeHero = 
                    (AudioPlayer_Hero.EAudioType_Hero)((int)AudioPlayer_Hero.EAudioType_Hero.SELECT_01 + Idx);
                AudioPlayer_Hero.instance.PlayAudio(audioTypeHero);
                break;
            case EObjectType.UNIT_01:
                AudioPlayer_Friendly_U01.EAudioType_Friendly_U01 audioTypeU01 = 
                    (AudioPlayer_Friendly_U01.EAudioType_Friendly_U01)((int)AudioPlayer_Friendly_U01.EAudioType_Friendly_U01.SELECT_01 + Idx);
                AudioPlayer_Friendly_U01.instance.PlayAudio(audioTypeU01);
                break;
            case EObjectType.UNIT_02:
                AudioPlayer_Friendly_U02.EAudioType_Friendly_U02 audioTypeU02 = 
                    (AudioPlayer_Friendly_U02.EAudioType_Friendly_U02)((int)AudioPlayer_Friendly_U02.EAudioType_Friendly_U02.SELECT_01 + Idx);
                AudioPlayer_Friendly_U02.instance.PlayAudio(audioTypeU02);
                break;
            case EObjectType.MAIN_BASE:
                AudioPlayer_Struct.instance.PlayAudio(AudioPlayer_Struct.EAudioType_Struct.MAIN_BASE);
                break;
            case EObjectType.NUCLEAR:
                AudioPlayer_Struct.instance.PlayAudio(AudioPlayer_Struct.EAudioType_Struct.NUCLEAR);
                break;
            case EObjectType.TURRET:
                AudioPlayer_Struct.instance.PlayAudio(AudioPlayer_Struct.EAudioType_Struct.TURRET);
                break;
            case EObjectType.BARRACK:
                AudioPlayer_Struct.instance.PlayAudio(AudioPlayer_Struct.EAudioType_Struct.BARRACK);
                break;
            case EObjectType.BUNKER:
                AudioPlayer_Struct.instance.PlayAudio(AudioPlayer_Struct.EAudioType_Struct.BUNKER);
                break;
            case EObjectType.WALL:
                AudioPlayer_Struct.instance.PlayAudio(AudioPlayer_Struct.EAudioType_Struct.WALL);
                break;
            default:
                break;
        }
    }
    
    
    public void PlayAudio_Advisor(EAudioType_Advisor _audioType)
    {
        int Idx = UnityEngine.Random.Range(0, 3); // Generates 0, 1, 2, or 3

        switch (_audioType)
        {
            case EAudioType_Advisor.UNDERATTACK:
                AudioPlayer_Advisor.EAudioType_Advisor audioTypeAdvisor = 
                    (AudioPlayer_Advisor.EAudioType_Advisor)((int)AudioPlayer_Advisor.EAudioType_Advisor.UNDERATTACK_01 + Idx);
                AudioPlayer_Advisor.instance.PlayAudio(audioTypeAdvisor);
                break;
            case EAudioType_Advisor.ENERGY:
                AudioPlayer_Advisor.instance.PlayAudio(AudioPlayer_Advisor.EAudioType_Advisor.ENERGY);
                break;
            case EAudioType_Advisor.CORE:
                AudioPlayer_Advisor.instance.PlayAudio(AudioPlayer_Advisor.EAudioType_Advisor.CORE);
                break;
            case EAudioType_Advisor.RESEARCH:
                AudioPlayer_Advisor.instance.PlayAudio(AudioPlayer_Advisor.EAudioType_Advisor.RESEARCH);
                break;
            case EAudioType_Advisor.UPGRADE:
                AudioPlayer_Advisor.instance.PlayAudio(AudioPlayer_Advisor.EAudioType_Advisor.UPGRADE);
                break;
            case EAudioType_Advisor.CONST_COMPLETE:
                AudioPlayer_Advisor.instance.PlayAudio(AudioPlayer_Advisor.EAudioType_Advisor.CONST_COMPLETE);
                break;
            case EAudioType_Advisor.CONST_CANCEL:
                AudioPlayer_Advisor.instance.PlayAudio(AudioPlayer_Advisor.EAudioType_Advisor.CONST_CANCEL);
                break;
            case EAudioType_Advisor.PAUSE:
                AudioPlayer_Advisor.instance.PlayAudio(AudioPlayer_Advisor.EAudioType_Advisor.PAUSE);
                break;
            case EAudioType_Advisor.RESUME:
                AudioPlayer_Advisor.instance.PlayAudio(AudioPlayer_Advisor.EAudioType_Advisor.RESUME);
                break;
            case EAudioType_Advisor.NUCLEAR_READY:
                AudioPlayer_Advisor.instance.PlayAudio(AudioPlayer_Advisor.EAudioType_Advisor.NUCLEAR_READY);
                break;
            case EAudioType_Advisor.NUCLEAR_LAUNCH:
                AudioPlayer_Advisor.instance.PlayAudio(AudioPlayer_Advisor.EAudioType_Advisor.NUCLEAR_LAUNCH);
                break;
            default:
                break;
        }
    }
    
    
    public void PlayAudio_Build(EObjectType _objectType)
    {
        AudioPlayer_Struct.instance.PlayAudio(AudioPlayer_Struct.EAudioType_Struct.BUILD);
    }
    
    public void PlayAudio_Destroy2(EObjectType _objectType)
    {
        AudioPlayer_Struct.instance.PlayAudio(AudioPlayer_Struct.EAudioType_Struct.DESTROY);
    }
    
    public void PlayAudio_Destroy(EObjectType _objectType)
    {
        switch (_objectType)
        {
            case EObjectType.UNIT_01:
                AudioPlayer_Friendly_U01.instance.PlayAudio(AudioPlayer_Friendly_U01.EAudioType_Friendly_U01.DEAD);
                break;
            case EObjectType.UNIT_02:
                AudioPlayer_Friendly_U02.instance.PlayAudio(AudioPlayer_Friendly_U02.EAudioType_Friendly_U02.DEAD);
                break;
            case EObjectType.UNIT_HERO:
                AudioPlayer_Hero.instance.PlayAudio(AudioPlayer_Hero.EAudioType_Hero.DEAD);
                break;
            case EObjectType.ENEMY_UNIT:
                AudioPlayer_Enemy.instance.PlayAudio(AudioPlayer_Enemy.EAudioType_Enemy.DEAD);
                break;
            case EObjectType.MAIN_BASE:
                AudioPlayer_Struct.instance.PlayAudio(AudioPlayer_Struct.EAudioType_Struct.DESTROY);
                break;
            case EObjectType.NUCLEAR:
                AudioPlayer_Struct.instance.PlayAudio(AudioPlayer_Struct.EAudioType_Struct.DESTROY);
                break;
            case EObjectType.TURRET:
                AudioPlayer_Struct.instance.PlayAudio(AudioPlayer_Struct.EAudioType_Struct.DESTROY);
                break;
            case EObjectType.BARRACK:
                AudioPlayer_Struct.instance.PlayAudio(AudioPlayer_Struct.EAudioType_Struct.DESTROY);
                break;
            case EObjectType.BUNKER:
                AudioPlayer_Struct.instance.PlayAudio(AudioPlayer_Struct.EAudioType_Struct.DESTROY);
                break;
            case EObjectType.WALL:
                AudioPlayer_Struct.instance.PlayAudio(AudioPlayer_Struct.EAudioType_Struct.DESTROY);
                break;
            default:
                break;
        }
    }
    
    public void PlayAudio_Misc(EAudioType_Misc _audioType)
    {
        int Idx = UnityEngine.Random.Range(0, 3); // Generates 0, 1, 2, or 3

        switch (_audioType)
        {
            case EAudioType_Misc.NUCLEAR_EXPLOSION:
                AudioPlayer_Misc.instance.PlayAudio(AudioPlayer_Misc.EAudioType_Misc.NUCLEAR_EXPLOSION);
                break;
            case EAudioType_Misc.PICKUP:
                AudioPlayer_Misc.instance.PlayAudio(AudioPlayer_Misc.EAudioType_Misc.PICKUP);
                break;
            default:
                break;
        }
    }
    
        
    public void PlayAudio_UI(EObjectType _objectType)
    {
        AudioPlayer_UI.instance.PlayAudio(AudioPlayer_UI.EAudioType_UI.CLICK);
    }
    
    public void PlayAudio_BGM()
    {
        AudioPlayer_BGM.instance.PlayAudio();
    }

    public void PlayAudio_BGM_WithFade(float _fadeDuration = 1.0f)
    {
        AudioPlayer_BGM.instance.PlayAudio();
    }
    
    public void StopAudio_BGM()
    {
        AudioPlayer_BGM.instance.StopAudio();
    }
    
    public void StopAudio_BGM_WithFade(float _fadeDuration = 1.0f)
    {
        AudioPlayer_BGM.instance.StopAudioWithFade(_fadeDuration);
    }
    
    public void PlayAudio_WaveBGM()
    {
        AudioPlayer_WaveBGM.instance.PlayAudio();
    }

    public void PlayAudio_WaveBGM_WithDelay(float delay = 1.0f)
    {
        AudioPlayer_WaveBGM.instance.PlayAudioWithDelay(delay);
    }
    
    public void StopAudio_WaveBGM()
    {
        AudioPlayer_WaveBGM.instance.StopAudio();
    }
    
    public void StopAudio_WaveBGM_WithFade(float _fadeDuration = 1.0f)
    {
        AudioPlayer_WaveBGM.instance.StopAudioWithFade(_fadeDuration);
    }

    public void PlayAudio_BGM_MainMenu()
    {
        AudioPlayer_BGM.instance.PlayAudio_MainMenu();
    }
    
    private void OnSceneLoaded(Scene _scene, LoadSceneMode _mode)
    {
        if (_scene.name.Equals("ProgrammingSceneMainMenu"))
        {
            AudioPlayer_BGM.instance.StopAudio();

            AudioPlayer_BGM.instance.PlayAudio_MainMenu();

            // instance.PlayAudio_BGM_MainMenu();


            //if (instance != null)
            //{
            //    // 게임화면 BGM 끄고
            //    AudioManager.instance.StopAudio_BGM();
            //}

            // if (instance == null) return;
            // 메인메뉴 BGM ON
            // Debug.Log("11");
            // AudioManager.instance.PlayAudio_BGM();

        }

        if (_scene.name.Equals("ProgrammingScene"))
        {
            // 메인메뉴 BGM STOP
            AudioPlayer_BGM.instance.StopAudio();

            // 게임 BGM ON
            AudioPlayer_BGM.instance.PlayAudio();
        }
        
    }
    
    public struct AudioVolumes
    {
        public float Main;
        public float BGM;
        public float Effect;
    }

    public AudioVolumes Volumes
    {
        get
        {
            return new AudioVolumes
            {
                Main = audioVolume_Master,
                BGM = audioVolume_BGM,
                Effect = audioVolume_Effect
            };
        }
    }
    
    
    public static AudioManager instance;
    
    [SerializeField] private float audioVolume_Master;
    [SerializeField] private float audioVolume_BGM;
    [SerializeField] private float audioVolume_Effect;
    
    
    // 모든 오디오 플레이어 인스턴스를 저장할 리스트
    private List<AudioPlayerBase> listBGMPlayer = new List<AudioPlayerBase>();
    private List<AudioPlayerBase> listEffectPlayer = new List<AudioPlayerBase>();

    private float sliderMaster = 0f;
    private float sliderBGM = 0f;
    private float sliderEffect = 0f;


    /*
    [Header("#BGM")]
    [SerializeField] private AudioClip bgmClip;
    [SerializeField] private float bgmVolume;
    private AudioSource bgmPlayer;
    */

}
