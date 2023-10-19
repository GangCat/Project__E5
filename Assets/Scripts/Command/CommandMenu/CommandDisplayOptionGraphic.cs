using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandDisplayOptionGraphic : Command
{
    public CommandDisplayOptionGraphic(CanvasMenuOptions _canvasOpt)
    {
        canvasOpt = _canvasOpt;
    }

    public override void Execute(params object[] _objects)
    {
        canvasOpt.HideAllOption();
        canvasOpt.DisplayGraphicOption();
    }

    private CanvasMenuOptions canvasOpt = null;
}
