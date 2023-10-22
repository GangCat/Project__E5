using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandAlertWaveStart : Command
{
    public CommandAlertWaveStart(CanvasAlert _canvas)
    {
        canvasAlert = _canvas;
    }

    public override void Execute(params object[] _objects)
    {
        canvasAlert.AlertWaveStart();
    }

    private CanvasAlert canvasAlert = null;
}
