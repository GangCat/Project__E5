using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandInitRemainEnemyCnt : Command
{
    public CommandInitRemainEnemyCnt(CanvasWaveInfo _canvas)
    {
        canvasWaveInfo = _canvas;
    }

    public override void Execute(params object[] _objects)
    {
        canvasWaveInfo.WaveStart((int)_objects[0]);
    }

    private CanvasWaveInfo canvasWaveInfo = null;
}
