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
                AudioManager.instance.PlayAudio_UI();
                ArrayPauseCommand.Use(EPauseCommand.TOGGLE_PAUSE);
            });

        btnOptions.onClick.AddListener(
            () =>
            {
                AudioManager.instance.PlayAudio_UI();
                ArrayMenuCommand.Use(EMenuCommand.DISPLAY_OPTION);
            });

        btnGameExit.onClick.AddListener(
            () =>
            {
                AudioManager.instance.PlayAudio_UI();
                reCheckGo.SetActive(true);
            });

        btnGameReturn.onClick.AddListener(
            () =>
            {
                AudioManager.instance.PlayAudio_UI();
                ArrayMenuCommand.Use(EMenuCommand.HIDE_MENU);
            });

        btnGameExitConfirm.onClick.AddListener(
            () =>
            {
                LoadSceneManager.ChangeScene("ProgrammingSceneMainMenu");
                //Application.Quit();
            });

        btnGameExitCancle.onClick.AddListener(
            () =>
            {
                AudioManager.instance.PlayAudio_UI();
                reCheckGo.SetActive(false);
            });

        reCheckGo.SetActive(false);
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
    [SerializeField]
    private GameObject reCheckGo = null;
    [SerializeField]
    private Button btnGameExitConfirm = null;
    [SerializeField]
    private Button btnGameExitCancle = null;
    
    private EObjectType objectType;
}
