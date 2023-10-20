using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    public void Init()
    {
        canvasMain = GetComponentInChildren<CanvasMainMenu>();
        canvasMainOption = GetComponentInChildren<CanvasMainOptions>();

        canvasMain.Init(DisplayOption);
        canvasMainOption.Init();
    }

    public void DisplayOption()
    {
        canvasMainOption.DisplayGraphicOption();
    }

    public void HideOption()
    {
        canvasMainOption.HideCanvas();
    }

    private CanvasMainMenu canvasMain = null;
    private CanvasMainOptions canvasMainOption = null;
}
