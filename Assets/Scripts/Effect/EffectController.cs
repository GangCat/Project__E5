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
        Debug.Log(_idx);
        arrEffect[_idx].DisplayEffect();
    }

    [SerializeField]
    protected EffectBase[] arrEffect = null;
}
