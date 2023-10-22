using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasGameClear : MonoBehaviour
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

    public void GameClear() 
    {
        gameObject.SetActive(true);
        ArrayPauseCommand.Use(EPauseCommand.TOGGLE_PAUSE);
    }

    [SerializeField]
    private Button btnConfirm = null;
}
