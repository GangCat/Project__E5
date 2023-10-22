using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandFriendlySignal : Command
{
    public CommandFriendlySignal(ImageMinimap _imageMinimap)
    {
        imageMinimap = _imageMinimap;
    }

    public override void Execute(params object[] _objects)
    {
        imageMinimap.FriendlySignal((Vector3)_objects[0]);
    }

    private ImageMinimap imageMinimap = null;
}
