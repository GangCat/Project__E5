using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandRemoveNuclearFromList : Command
{
    public CommandRemoveNuclearFromList(StructureManager _mng)
    {
        structureMng = _mng;
    }
    public override void Execute(params object[] _objects)
    {
        structureMng.RemoveNuclearAtList((StructureNuclear)_objects[0]);
    }

    private StructureManager structureMng = null;
}
