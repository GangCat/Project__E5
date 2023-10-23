using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasMainMenu : MonoBehaviour
{
    public void Init(VoidVoidDelegate _displayOptionCallback)
    {
        buttonStartGame.onClick.AddListener(
            () =>
            {
                AudioManager.instance.PlayAudio_UI(objectType);
                LoadSceneManager.ChangeScene("ProgrammingScene");
            });

        buttonOption.onClick.AddListener(
            () =>
            {
                AudioManager.instance.PlayAudio_UI(objectType);
                _displayOptionCallback?.Invoke();
            });

        buttonExit.onClick.AddListener(
            () =>
            {
                AudioManager.instance.PlayAudio_UI(objectType);
                DisplayExitRecheckButton();
            });

        buttonExitConfrim.onClick.AddListener(
            () =>
            {
                Application.Quit();
            });

        buttonExitCancle.onClick.AddListener(
            () =>
            {
                AudioManager.instance.PlayAudio_UI(objectType);
                exitReCheckGo.SetActive(false);
            });


        exitReCheckGo.gameObject.SetActive(false);
    }

    private void DisplayExitRecheckButton()
    {
        exitReCheckGo.gameObject.SetActive(true);
    }


    [SerializeField]
    private Button buttonStartGame = null;
    [SerializeField]
    private Button buttonOption = null;
    [SerializeField]
    private Button buttonExit = null;
    [SerializeField]
    private GameObject exitReCheckGo = null;
    [SerializeField]
    private Button buttonExitConfrim = null;
    [SerializeField]
    private Button buttonExitCancle = null;

    private EObjectType objectType;
}
