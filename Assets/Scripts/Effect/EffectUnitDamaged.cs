using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectUnitDamaged : EffectBase
{
    public override void DisplayEffect()
    {
        if (!isActive)
        {
            isActive = true;
            gameObject.SetActive(true);
            mr.SetInt("_isDamaged", 1);
            Invoke("DisableObject", activeTime);
        }
    }

    protected override void DisableObject()
    {
        mr.SetInt("_isDamaged", 0);
        base.DisableObject();
    }

    [SerializeField]
    private Material mr = null;
}
