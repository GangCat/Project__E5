using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NuclearMissileEffectController : EffectController
{
    public void EffectOn(int _idx, bool _effect)
    {
        if (_effect)
        {
            arrEffect[_idx].DisplayEffect();
            arrEffect[_idx].transform.parent = null;
        }
    }
}
