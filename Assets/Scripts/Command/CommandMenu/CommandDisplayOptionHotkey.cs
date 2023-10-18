using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandDisplayOptionHotkey : Command
{
    public CommandDisplayOptionHotkey(CanvasMenuOptions _canvasOpt)
    {
        canvasOpt = _canvasOpt;
    }

    public override void Execute(params object[] _objects)
    {
        canvasOpt.HideAllOption();
        canvasOpt.DisplayHotkeyOption();
    }

    private CanvasMenuOptions canvasOpt = null;
}
