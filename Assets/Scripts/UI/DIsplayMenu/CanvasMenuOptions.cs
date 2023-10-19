using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasMenuOptions : CanvasBase
{
    public void Init()
    {
        buttonGraphic.onClick.AddListener(
            () =>
            {
                ArrayMenuCommand.Use(EMenuCommand.DISPLAY_OPTION_GRAPHIC);
            });

        buttonSound.onClick.AddListener(
            () =>
            {
                ArrayMenuCommand.Use(EMenuCommand.DISPLAY_OPTION_SOUND);
            });

        buttonHotkey.onClick.AddListener(
            () =>
            {
                ArrayMenuCommand.Use(EMenuCommand.DISPLAY_OPTION_HOTKEY);
            });

        buttonReturnMenu.onClick.AddListener(
            () =>
            {
                ArrayMenuCommand.Use(EMenuCommand.RETURN_MENU);
            });

        imageGraphic.Init();
        imageSound.Init();
        imageHotkey.Init();

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
        imageHotkey.DisplayImage();
    }

    public void HideAllOption()
    {
        imageGraphic.HideImage();
        imageSound.HideImage();
        imageHotkey.HideImage();
    }


    [SerializeField]
    private Button buttonGraphic = null;
    [SerializeField]
    private Button buttonSound = null;
    [SerializeField]
    private Button buttonHotkey = null;
    [SerializeField]
    private Button buttonReturnMenu = null;
    [SerializeField]
    private MenuImageBase imageGraphic = null;
    [SerializeField]
    private MenuImageBase imageSound = null;
    [SerializeField]
    private MenuImageBase imageHotkey = null;
}
