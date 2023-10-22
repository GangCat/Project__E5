using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageAlertBox : MonoBehaviour
{
    public void Init()
    {
        text.Init();
        SetActive(false);
    }

    public void Display(string _text)
    {
        text.UpdateText(_text);
        SetActive(true);
    }

    public void Hide()
    {
        SetActive(false);
    }

    private void SetActive(bool _isActive)
    {
        gameObject.SetActive(_isActive);
    }

    [SerializeField]
    private TextAlert text = null;
}
