using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandAlertBuildComplete : Command
{
    public CommandAlertBuildComplete(CanvasAlert _canvas)
    {
        canvasAlert = _canvas;
    }

    public override void Execute(params object[] _objects)
    {
        canvasAlert.AlertBuildComplete();
    }

    private CanvasAlert canvasAlert = null;
}
