using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandBigEnemySignal : Command
{
    public CommandBigEnemySignal(ImageMinimap _imageMinimap)
    {
        imageMinimap = _imageMinimap;
    }

    public override void Execute(params object[] _objects)
    {
        Transform[] arrTr = (Transform[])_objects;
        imageMinimap.BigEnemySignal(arrTr);
    }

    private ImageMinimap imageMinimap = null;
}
