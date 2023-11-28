using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FollowObjText : MonoBehaviour
{
    public GameObject followTarget;

    private void Update()
    {
        transform.position = Camera.main.WorldToScreenPoint(followTarget.transform.position);
    }

    public void UpdateText(string _str)
    {
        GetComponent<TMP_Text>().text = _str;
    }
}
