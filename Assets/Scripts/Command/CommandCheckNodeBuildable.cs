using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandCheckNodeBuildable : Command
{
    public CommandCheckNodeBuildable(PF_PathRequestManager _pathFinding)
    {
        pathMng = _pathFinding;
    }

    public override void Execute(params object[] _objects)
    {
        PF_Node[] nodes = (PF_Node[])_objects;
        pathMng.CheckNodeBuildable(nodes);
    }

    private PF_PathRequestManager pathMng = null;
}
