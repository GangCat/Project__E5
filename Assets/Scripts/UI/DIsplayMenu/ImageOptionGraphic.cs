using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageOptionGraphic : MenuImageBase
{
    public void Init(bool _isFullHD, bool _isFullScreen)
    {
        toggleFullHD.onValueChanged.AddListener(
            (bool _value) =>
            {
                if (_value.Equals(GameManager.IsFullHD))
                    return;
                AudioManager.instance.PlayAudio_UI(objectType);
                GameManager.ChangeDisplayFullHD(_value);
            });

        toggleFullScreen.onValueChanged.AddListener(
            (bool _value) =>
            {
                if (_value.Equals(GameManager.IsFullScreen))
                    return;
                AudioManager.instance.PlayAudio_UI(objectType);
                GameManager.ToggleFullscreen(_value);
            });

        toggleFullHD.isOn = _isFullHD;
        toggleFullScreen.isOn = _isFullScreen;

    }

    [SerializeField]
    private Toggle toggleFullScreen = null;
    [SerializeField]
    private Toggle toggleFullHD = null;
}
