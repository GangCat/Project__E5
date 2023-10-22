using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    public void Init(bool _isFullHD, bool _isFullScreen)
    {
        canvasMain = GetComponentInChildren<CanvasMainMenu>();
        canvasMainOption = GetComponentInChildren<CanvasMainOptions>();

        canvasMain.Init(DisplayOption);
        canvasMainOption.Init(_isFullHD, _isFullScreen);
    }

    public void DisplayOption()
    {
        canvasMainOption.DisplayCanvas();
    }

    public void HideOption()
    {
        canvasMainOption.HideCanvas();
    }

    private CanvasMainMenu canvasMain = null;
    private CanvasMainOptions canvasMainOption = null;
}
