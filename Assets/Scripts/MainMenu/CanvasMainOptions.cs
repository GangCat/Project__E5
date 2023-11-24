using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasMainOptions : CanvasMenuOptions
{
    public override void Init(bool _isFullHD, bool _isFullScreen)
    {
        buttonGraphic.onClick.AddListener(
            () =>
            {
                AudioManager.instance.PlayAudio_UI();
                HideAllOption();
                DisplayGraphicOption();
            });

        buttonSound.onClick.AddListener(
            () =>
            {
                AudioManager.instance.PlayAudio_UI();
                HideAllOption();
                DisplaySoundOption();
            });

        buttonReturnMenu.onClick.AddListener(
            () =>
            {
                AudioManager.instance.PlayAudio_UI();
                SetActive(false);
            });

        imageGraphic.Init(_isFullHD, _isFullScreen);
        imageSound.Init();

        SetActive(false);
    }
}
