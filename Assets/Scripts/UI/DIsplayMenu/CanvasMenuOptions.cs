using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasMenuOptions : CanvasBase
{
    public virtual void Init(bool _isFullHD, bool _isFullScreen)
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

        //buttonHotkey.onClick.AddListener(
        //    () =>
        //    {
        //        ArrayMenuCommand.Use(EMenuCommand.DISPLAY_OPTION_HOTKEY);
        //    });

        buttonReturnMenu.onClick.AddListener(
            () =>
            {
                AudioManager.instance.PlayAudio_UI(objectType);
                ArrayMenuCommand.Use(EMenuCommand.RETURN_MENU);
            });

        //imageHotkey.Init();

        imageGraphic.Init(_isFullHD, _isFullScreen);
        imageSound.Init();

        SetActive(false);
    }

    public override void DisplayCanvas()
    {
        HideAllOption();
        DisplayGraphicOption();
        base.DisplayCanvas();
    }

    public void DisplayGraphicOption()
    {
        imageGraphic.DisplayImage();
    }

    public void DisplaySoundOption()
    {
        imageSound.DisplayImage();
    }

    public void DisplayHotkeyOption()
    {
        //imageHotkey.DisplayImage();
    }

    public void HideAllOption()
    {
        imageGraphic.HideImage();
        imageSound.HideImage();
        //imageHotkey.HideImage();
    }


    [SerializeField]
    protected Button buttonGraphic = null;
    [SerializeField]
    protected Button buttonSound = null;
    //[SerializeField]
    //private Button buttonHotkey = null;
    [SerializeField]
    protected Button buttonReturnMenu = null;
    [SerializeField]
    protected ImageOptionGraphic imageGraphic = null;
    [SerializeField]
    protected ImageOptionSound imageSound = null;
    //[SerializeField]
    //private MenuImageBase imageHotkey = null;

    protected EObjectType objectType;
}
