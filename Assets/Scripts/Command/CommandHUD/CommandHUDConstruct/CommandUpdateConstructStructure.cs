using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CommandUpdateConstructStructure : Command
{
    public CommandUpdateConstructStructure(CanvasConstructInfo _canvas)
    {
        canvasConstruct = _canvas;
    }

    public override void Execute(params object[] _objects)
    {
        canvasConstruct.UpdateUnit((string)_objects[0], (EObjectType)_objects[1]);
    }

    private CanvasConstructInfo canvasConstruct = null;
}
