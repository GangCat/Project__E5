using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageOptionHotkey : MenuImageBase
{
    public override void Init()
    {
        for(int i = 0; i < arrUnitFuncHotkey.Length; ++i)
        {
            arrUnitFuncHotkey[i].onClick.AddListener(
                () =>
                {
                    ArrayChangeHotkeyCommand.Use(EChangeHotkeyCommand.SELECT_UNIT_FUNC_BUTTON, (EUnitFuncKey)i);
                });
        }

        for(int i = 0; i < arrStructureFuncHotkey.Length; ++i)
        {
            arrStructureFuncHotkey[i].onClick.AddListener(
                () =>
                {
                    ArrayChangeHotkeyCommand.Use(EChangeHotkeyCommand.SELECT_STRUCTURE_FUNC_BUTTON, (EStructureFuncKey)i);
                });
        }

        for(int i = 0; i < arrBarrackFuncHotkey.Length; ++i)
        {
            arrBarrackFuncHotkey[i].onClick.AddListener(
                () =>
                {
                    ArrayChangeHotkeyCommand.Use(EChangeHotkeyCommand.SELECT_BARRACK_FUNC_BUTTON, (EBarrackFuncKey)i);
                });
        }

        for(int i = 0; i < arrBuildFuncHotkey.Length; ++i)
        {
            arrBuildFuncHotkey[i].onClick.AddListener(
                () =>
                {
                    ArrayChangeHotkeyCommand.Use(EChangeHotkeyCommand.SELECT_BUILD_FUNC_BUTTON, (EBuildFuncKey)i);
                });
        }
    }

    [SerializeField]
    private Button[] arrUnitFuncHotkey = null;
    [SerializeField]
    private Button[] arrStructureFuncHotkey = null;
    [SerializeField]
    private Button[] arrBarrackFuncHotkey = null;
    [SerializeField]
    private Button[] arrBuildFuncHotkey = null;
}
