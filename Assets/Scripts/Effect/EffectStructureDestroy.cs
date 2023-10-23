using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectStructureDestroy : EffectBase
{
    public void Init(Vector3 _pos, VoidTransformDelegate _deactivateCallback)
    {
        transform.position = _pos;
        deactivateCallback = _deactivateCallback;
        DisplayEffect();
    }

    public override void DisplayEffect()
    {
        isActive = true;
        Invoke("DisableObject", activeTime);
    }

    public override void DisableObject()
    {
        deactivateCallback?.Invoke(transform);
    }

    private VoidTransformDelegate deactivateCallback = null;
}
