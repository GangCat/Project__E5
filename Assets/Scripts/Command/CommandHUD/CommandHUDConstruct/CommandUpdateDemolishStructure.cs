using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandUpdateDemolishStructure : Command
{
    public CommandUpdateDemolishStructure(CanvasDemolishInfo _canvasDemolish)
    {
        canvasDemolish = _canvasDemolish;
    }
    public override void Execute(params object[] _objects)
    {
        canvasDemolish.UpdateUnit((string)_objects[0], (EObjectType)_objects[1]);
    }

    private CanvasDemolishInfo canvasDemolish = null;
}
