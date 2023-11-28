using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TempButton : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(
            () =>
            {
                ArrayChangeHotkeyCommand.Use(EChangeHotkeyCommand.SELECT_UNIT_FUNC_BUTTON, EUnitFuncKey.ATTACK);
                keyChangeImage.SetActive(true);
            });
    }

    public GameObject keyChangeImage;

}