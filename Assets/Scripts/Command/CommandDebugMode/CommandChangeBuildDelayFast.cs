using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandChangeBuildDelayFast : Command
{
    public CommandChangeBuildDelayFast(StructureManager _structureMng)
    {
        structureMng = _structureMng;
    }

    public override void Execute(params object[] _objects)
    {
        structureMng.SetBuildDelayFast();
    }

    private StructureManager structureMng = null;
}
