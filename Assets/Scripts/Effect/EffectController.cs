using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectController : MonoBehaviour
{
    public virtual void Init()
    {
        for(int i = 0; i < arrEffect.Length; ++i)
            arrEffect[i].Init();
    }

    public virtual void EffectOn(int _idx)
    {
        arrEffect[_idx].DisplayEffect();
    }

    public virtual void EffectOn(int _idx, Vector3 _pos)
    {
        arrEffect[_idx].DisplayEffect(_pos);
    }

    public virtual void EffectOff(int _idx)
    {
        arrEffect[_idx].DisableObject();
    }

    [SerializeField]
    protected EffectBase[] arrEffect = null;
}
