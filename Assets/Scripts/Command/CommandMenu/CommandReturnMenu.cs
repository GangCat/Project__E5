using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandReturnMenu : Command
{
    public CommandReturnMenu(CanvasMenuOptions _canvasOpt, CanvasMenuPopup _canvasMenu)
    {
        canvasOpt = _canvasOpt;
        canvasMenu = _canvasMenu;
    }

    public override void Execute(params object[] _objects)
    {
        canvasOpt.HideCanvas();
        canvasMenu.DisplayCanvas();
    }

    private CanvasMenuOptions canvasOpt = null;
    private CanvasMenuPopup canvasMenu = null;
}
