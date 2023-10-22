using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandAlertUpgradeComplete : Command
{
    public CommandAlertUpgradeComplete(CanvasAlert _canvas)
    {
        canvasAlert = _canvas;
    }

    public override void Execute(params object[] _objects)
    {
        canvasAlert.AlertUpgradeComplete();
    }

    private CanvasAlert canvasAlert = null;
}
