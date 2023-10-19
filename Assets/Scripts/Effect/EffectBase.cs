using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectBase : MonoBehaviour
{
    public virtual void Init()
    {
        gameObject.SetActive(false);
    }

    public virtual void DisplayEffect()
    {
        if (!isActive)
        {
            isActive = true;
            gameObject.SetActive(true);
            Invoke("DisableObject", activeTime);
        }
    }

    protected virtual void DisableObject()
    {
        isActive = false;
        gameObject.SetActive(false);
    }

    protected bool isActive = false;
    [SerializeField]
    protected float activeTime = 0f;
}