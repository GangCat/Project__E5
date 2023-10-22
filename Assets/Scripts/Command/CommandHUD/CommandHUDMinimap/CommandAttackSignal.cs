using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandAttackSignal : Command
{
    public CommandAttackSignal(ImageMinimap _imageMinimap)
    {
        imageMinimap = _imageMinimap;
    }
    public override void Execute(params object[] _objects)
    {
        imageMinimap.AttackSignal((Vector3)_objects[0]);
    }

    private ImageMinimap imageMinimap = null;
}
