using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempChangeImage : MonoBehaviour
{
    private void Start()
    {
        gameObject.SetActive(false);
    }

    private void Update()
    {
        if (Input.anyKey)
            gameObject.SetActive(false);
    }
}
