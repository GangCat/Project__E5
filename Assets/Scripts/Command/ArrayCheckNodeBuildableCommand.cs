using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrayCheckNodeBuildableCommand
{
    private static Command[] arrCmd = new Command[(int)ECheckNodeBuildable.LENGTH];

    public static void Add(ECheckNodeBuildable _eCmd, Command _cmd)
    {
        arrCmd[(int)_eCmd] = _cmd;
    }

    public static void Use(ECheckNodeBuildable _eCmd, params object[] _objects)
    {
        arrCmd[(int)_eCmd].Execute(_objects);
    }
}
