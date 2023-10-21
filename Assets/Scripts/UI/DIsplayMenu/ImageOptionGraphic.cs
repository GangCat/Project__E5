using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageOptionGraphic : MenuImageBase
{
    public override void Init()
    {
        toggleFullHD.onValueChanged.AddListener(
            (bool _value) =>
            {
                AudioManager.instance.PlayAudio_UI(objectType);
                GameManager.ChangeDisplayFullHD(_value);
            });

        toggleFullScreen.onValueChanged.AddListener(
            (bool _value) =>
            {
                AudioManager.instance.PlayAudio_UI(objectType);
                GameManager.ToggleFullscreen(_value);
            });
    }

    [SerializeField]
    private Toggle toggleFullScreen = null;
    [SerializeField]
    private Toggle toggleFullHD = null;
    
}
