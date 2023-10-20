using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasMainOptions : CanvasMenuOptions
{
    public override void Init()
    {
        buttonGraphic.onClick.AddListener(
            () =>
            {
                AudioManager.instance.PlayAudio_UI(objectType);
                HideAllOption();
                DisplayGraphicOption();
            });

        buttonSound.onClick.AddListener(
            () =>
            {
                AudioManager.instance.PlayAudio_UI(objectType);
                HideAllOption();
                DisplaySoundOption();
            });

        buttonReturnMenu.onClick.AddListener(
            () =>
            {
                AudioManager.instance.PlayAudio_UI(objectType);
                SetActive(false);
            });

        SetActive(false);
    }
}
