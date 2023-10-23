using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandWaveFinish : Command
{
    public CommandWaveFinish(CanvasWaveInfo _canvas)
    {
        canvasWaveInfo = _canvas;
    }
    public override void Execute(params object[] _objects)
    {
        canvasWaveInfo.WaveEnd();
    }

    private CanvasWaveInfo canvasWaveInfo = null;
}
