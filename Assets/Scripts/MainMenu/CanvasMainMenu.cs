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

        buttonExitReCheck.onClick.AddListener(
            () =>
            {
                Application.Quit();
                buttonExitReCheck.gameObject.SetActive(false);
            });

        buttonExitReCheck.gameObject.SetActive(false);
    }

    private void DisplayExitRecheckButton()
    {
        buttonExitReCheck.gameObject.SetActive(true);
    }


    [SerializeField]
    private Button buttonStartGame = null;
    [SerializeField]
    private Button buttonOption = null;
    [SerializeField]
    private Button buttonExit = null;
    [SerializeField]
    private Button buttonExitReCheck = null;
}
