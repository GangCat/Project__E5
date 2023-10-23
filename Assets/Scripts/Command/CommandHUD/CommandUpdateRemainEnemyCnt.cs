using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandUpdateRemainEnemyCnt : Command
{
    public CommandUpdateRemainEnemyCnt(CanvasWaveInfo _canvas)
    {
        canvasWaveInfo = _canvas;
    }

    public override void Execute(params object[] _objects)
    {
        canvasWaveInfo.UpdateRemainEnemyCnt((int)_objects[0]);
    }

    private CanvasWaveInfo canvasWaveInfo = null;
}
