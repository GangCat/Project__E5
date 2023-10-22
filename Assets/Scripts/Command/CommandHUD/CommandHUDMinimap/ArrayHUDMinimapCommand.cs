using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrayHUDMinimapCommand
{
    private static Command[] arrCmd = new Command[(int)EHUDMinimapCommand.LENGTH];

    public static void Add(EHUDMinimapCommand _eCmd, Command _cmd)
    {
        arrCmd[(int)_eCmd] = _cmd;
    }

    public static void Use(EHUDMinimapCommand _eCmd, params object[] _objects)
    {
        arrCmd[(int)_eCmd].Execute(_objects);
    }
}
