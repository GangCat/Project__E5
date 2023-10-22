using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandToggleDisplayFog : Command
{
    public CommandToggleDisplayFog(FogManager _fogMng)
    {
        fogMng = _fogMng;
    }

    public override void Execute(params object[] _objects)
    {
        fogMng.ToggleFogCombineVisible();
    }

    private FogManager fogMng = null;
}
