using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageOptionSound : MenuImageBase
{
    public override void Init()
    {
        instance = AudioManager.instance;

        sliderMaster.onValueChanged.AddListener(
            (float _volume) =>
            {
                instance.SetMasterVolume(_volume);
            });

        sliderBGM.onValueChanged.AddListener(
            (float _volume) =>
            {
                instance.SetBGMVolume(_volume);
            });

        sliderEffect.onValueChanged.AddListener(
            (float _volume) =>
            {
                instance.SetEffectVolume(_volume);
            });

        sliderMaster.value = instance.CurMasterVolume;
        sliderEffect.value = instance.CurEffectVolume;
        sliderBGM.value = instance.CurBGMVolume;

        SetActive(false);
    }


    [SerializeField]
    private Slider sliderBGM = null;
    [SerializeField]
    private Slider sliderEffect = null;
    [SerializeField]
    private Slider sliderMaster = null;

    private AudioManager instance = null;
}
