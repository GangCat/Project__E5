using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasMenuPopup : CanvasBase
{
    public void Init()
    {
        btnPause.onClick.AddListener(
            () =>
            {
                AudioManager.instance.PlayAudio_UI(objectType);
                ArrayPauseCommand.Use(EPauseCommand.TOGGLE_PAUSE);
            });

        btnOptions.onClick.AddListener(
            () =>
            {
                AudioManager.instance.PlayAudio_UI(objectType);
                ArrayMenuCommand.Use(EMenuCommand.DISPLAY_OPTION);
            });


        btnGameReturn.onClick.AddListener(
            () =>
            {
                AudioManager.instance.PlayAudio_UI(objectType);
                ArrayMenuCommand.Use(EMenuCommand.HIDE_MENU);
            });

        SetActive(false);
    }

    [SerializeField]
    private Button btnPause = null;
    [SerializeField]
    private Button btnOptions = null;
    [SerializeField]
    private Button btnGameExit = null;
    [SerializeField]
    private Button btnGameReturn = null;
    
    private EObjectType objectType;
}
