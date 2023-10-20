using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageOptionSound : MenuImageBase
{
    public override void Init()
    {
        btnSFXTestPlay.Init();
    }

    [SerializeField]
    private Slider sliderMasterVolume = null;
    [SerializeField]
    private Slider sliderBackgroundMusicVolume = null;
    [SerializeField]
    private Slider sliderSoundEffectVolume = null;

    [SerializeField]
    private ButtonSFXTestPlay btnSFXTestPlay = null;
}
