using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StructureEffectController : EffectController
{
    public void EffectOn(int _idx, Vector3 _pos)
    {
        Debug.Log(_idx);
        arrEffect[_idx].DisplayEffect(_pos);
    }
}
