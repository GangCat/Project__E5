using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public void Init(bool _isFullHD, bool _isFullScreen)
    {
        funcBtnMng = GetComponentInChildren<FuncButtonManager>();
        displayHUDMng = GetComponentInChildren<DisplayHUDManager>();
        displayMenuMng = GetComponentInChildren<DisplayMenuManager>();
        displayCurMng = GetComponentInChildren<DisplayCurrencyManager>();
        canvasGameOver = GetComponentInChildren<CanvasGameOver>();
        canvasGameClear = GetComponentInChildren<CanvasGameClear>();

        funcBtnMng.Init();
        displayHUDMng.Init();
        displayMenuMng.Init(_isFullHD, _isFullScreen);
        displayCurMng.Init();
        canvasGameOver.Init();
        canvasGameClear.Init();
    }

    public void HeroDead()
    {
        displayHUDMng.HeroDead();
    }

    public void ShowFuncButton(EObjectType _selectObjectType)
    {
        funcBtnMng.ShowFuncButton(_selectObjectType);
    }

    public void UpdateEnergy(uint _curEnergy)
    {
        displayCurMng.UpdateEnergy(_curEnergy);
    }

    public void UpdateCore(uint _curCore)
    {
        displayCurMng.UpdateCore(_curCore);
    }

    public void UpdateCurPopulation(uint _curPopulation)
    {
        displayCurMng.UpdateCurPopulation(_curPopulation);
    }

    public void UpdateCurMaxPopulation(uint _curMaxPopulation)
    {
        displayCurMng.UpdateCurMaxPopulation(_curMaxPopulation);
    }

    public static void GameClear()
    {
        canvasGameClear.GameClear();
    }

    public static void GameOver()
    {
        canvasGameOver.GameOver();
    }

    private FuncButtonManager funcBtnMng = null;
    private DisplayHUDManager displayHUDMng = null;
    private DisplayMenuManager displayMenuMng = null;
    private DisplayCurrencyManager displayCurMng = null;
    private static CanvasGameOver canvasGameOver = null;
    private static CanvasGameClear canvasGameClear = null;

}
