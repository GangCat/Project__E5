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

    public void ChangeMasterVolume(float _volume)
    {
        AudioManager.instance.SetMasterVolume(_volume);
    }
    
    public void ChangeEffectVolume(float _volume)
    {
        AudioManager.instance.SetEffectVolume(_volume);
    }
    
    public void ChangeBGMVolume(float _volume)
    {
        AudioManager.instance.SetBGMVolume(_volume);
    }

    [SerializeField]
    private ButtonSFXTestPlay btnSFXTestPlay = null;
}
