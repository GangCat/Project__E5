using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectCollectCore : EffectBase
{
    public override void Init()
    {
    }

    public override void DisplayEffect(Vector3 _pos)
    {
        GameObject effectGo = Instantiate(effect, _pos, Quaternion.identity);
        Destroy(effectGo, activeTime);
    }

    [SerializeField]
    private GameObject effect = null;
}
