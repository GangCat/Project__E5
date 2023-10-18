using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasPause : CanvasBase, IPauseObserver
{
    public void Init()
    {
        ArrayPauseCommand.Use(EPauseCommand.REGIST, this);
        HideCanvas();
    }

    public void CheckPause(bool _isPause)
    {
        SetActive(_isPause);
    }

}
