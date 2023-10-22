using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasGameOver : MonoBehaviour
{
    public void Init()
    {
        btnConfirm.onClick.AddListener(
            () =>
            {
                LoadSceneManager.ChangeScene("ProgrammingSceneMainMenu");
            });

        gameObject.SetActive(false);
    }

    public void GameOver()
    {
        gameObject.SetActive(true);
        ArrayPauseCommand.Use(EPauseCommand.TOGGLE_PAUSE);
    }


    [SerializeField]
    private Button btnConfirm = null;
}
