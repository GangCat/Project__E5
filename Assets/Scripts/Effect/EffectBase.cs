using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectBase : MonoBehaviour
{
    public void DisplayEffect()
    {
        if (!isActive)
        {
            isActive = true;
            gameObject.SetActive(true);
            StartCoroutine("AutoDisableCoroutine");
        }
    }

    private IEnumerator AutoDisableCoroutine()
    {
        yield return new WaitForSeconds(activeTime);
        isActive = false;
        gameObject.SetActive(false);
    }

    private bool isActive = false;
    [SerializeField]
    private float activeTime = 0f;
}