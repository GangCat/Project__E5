using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayMenuManager : MonoBehaviour
{
    public void Init(bool _isFullHD, bool _isFullScreen)
    {
        canvasMenuPopup = GetComponentInChildren<CanvasMenuPopup>();
        canvasMenuOptions = GetComponentInChildren<CanvasMenuOptions>();
        canvasPause = GetComponentInChildren<CanvasPause>();

        canvasPause.Init();
        canvasMenuPopup.Init();
        canvasMenuOptions.Init(_isFullHD, _isFullScreen);

        ArrayMenuCommand.Add(EMenuCommand.DISPLAY_MENU, new CommandDisplayMenu(canvasMenuPopup));
        ArrayMenuCommand.Add(EMenuCommand.HIDE_MENU, new CommandHideMenu(canvasMenuPopup));
        ArrayMenuCommand.Add(EMenuCommand.DISPLAY_OPTION, new CommandDisplayOption(canvasMenuOptions, canvasMenuPopup));
        ArrayMenuCommand.Add(EMenuCommand.RETURN_MENU, new CommandReturnMenu(canvasMenuOptions, canvasMenuPopup));
    }

    private CanvasPause canvasPause = null;
    private CanvasMenuPopup canvasMenuPopup = null;
    private CanvasMenuOptions canvasMenuOptions = null;
}
