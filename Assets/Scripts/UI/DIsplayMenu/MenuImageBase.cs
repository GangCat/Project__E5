using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuImageBase : MonoBehaviour
{
    public virtual void Init()
    {
        
    }

    public void DisplayImage()
    {
        SetActive(true);
    }

    public void HideImage()
    {
        SetActive(false);
    }

    protected void SetActive(bool _isActive)
    {
        gameObject.SetActive(_isActive);
    }

    protected EObjectType objectType;
}
