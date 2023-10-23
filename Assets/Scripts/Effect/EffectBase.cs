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

    public virtual void DisplayEffect(Vector3 _pos)
    {
        if (!isActive)
        {
            isActive = true;
            transform.position = _pos;
            gameObject.SetActive(true);
            Invoke("DisableObject", activeTime);
        }
    }

    public virtual void DisableObject()
    {
        if (isActive)
        {
            isActive = false;
            gameObject.SetActive(false);
        }
    }

    [SerializeField]
    protected float activeTime = 0f;

    protected bool isActive = false;
}