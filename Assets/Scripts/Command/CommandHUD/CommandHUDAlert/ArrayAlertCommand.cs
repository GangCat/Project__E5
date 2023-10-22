using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrayAlertCommand
{
    private static Command[] arrCmd = new Command[(int)EAlertCommand.LENGTH];

    public static void Add(EAlertCommand _eCmd, Command _cmd)
    {
        arrCmd[(int)_eCmd] = _cmd;
    }

    public static void Use(EAlertCommand _eCmd, params object[] _objects)
    {
        arrCmd[(int)_eCmd].Execute(_objects);
    }
}
