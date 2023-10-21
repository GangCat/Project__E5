using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugModeManager : MonoBehaviour
{
    public void Init(StructureManager _structureMng)
    {
        canvasDebug = GetComponentInChildren<CanvasDebugMode>();
        canvasDebug.Init();
        structureMng = _structureMng;
        SetActive(false);
    }

    public bool isActive => gameObject.activeSelf;
    
    public void SetActive(bool _isActive)
    {
        gameObject.SetActive(_isActive);
    }

    public void DisplayCurState(Vector3 _screenPos, EState _curState)
    {
        canvasDebug.DisplayCurState(_screenPos, _curState);
    }

    public void BuildDelayFastChect()
    {
        structureMng.SetBuildDelayFast();
    }

    private CanvasDebugMode canvasDebug = null;
    private StructureManager structureMng = null;
}
