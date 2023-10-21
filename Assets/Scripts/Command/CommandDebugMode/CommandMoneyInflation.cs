using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandMoneyInflation : Command
{
    public CommandMoneyInflation(CurrencyManager _curMng)
    {
        curMng = _curMng;
    }

    public override void Execute(params object[] _objects)
    {
        curMng.MoneyInflation();
    }

    private CurrencyManager curMng = null;
}
