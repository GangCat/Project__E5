using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandDisplayOption : Command
{
    public CommandDisplayOption(CanvasMenuOptions _canvasOpt, CanvasMenuPopup _canvasMenu)
    {
        canvasOpt = _canvasOpt;
        canvasMenu = _canvasMenu;
    }

    public override void Execute(params object[] _objects)
    {
        canvasMenu.HideCanvas();
        canvasOpt.DisplayCanvas();
    }

    private CanvasMenuPopup canvasMenu = null;
    private CanvasMenuOptions canvasOpt = null;
}
