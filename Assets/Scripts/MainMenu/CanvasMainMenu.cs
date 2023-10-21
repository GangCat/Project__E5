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
                LoadSceneManager.ChangeScene("ProgrammingScene");
            });

        buttonOption.onClick.AddListener(
            () =>
            {
                _displayOptionCallback?.Invoke();
            });

        buttonExit.onClick.AddListener(
            () =>
            {
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
}
