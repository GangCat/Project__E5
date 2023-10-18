using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandDisplayOptionSound : Command
{
    public CommandDisplayOptionSound(CanvasMenuOptions _canvasOpt)
    {
        canvasOpt = _canvasOpt;
    }
    public override void Execute(params object[] _objects)
    {
        canvasOpt.HideAllOption();
        canvasOpt.DisplaySoundOption();
    }

    private CanvasMenuOptions canvasOpt = null;
}
