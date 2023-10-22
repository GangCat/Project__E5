using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandAlertUnderAttack : Command
{
    public CommandAlertUnderAttack(CanvasAlert _canvas)
    {
        canvasAlert = _canvas;
    }

    public override void Execute(params object[] _objects)
    {
        canvasAlert.AlertUnderAttack();
    }

    private CanvasAlert canvasAlert = null;
}
