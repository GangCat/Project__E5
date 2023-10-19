using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectCoreCollider : MonoBehaviour
{
    public void Init(EffectController _effectCtrl)
    {
        effectCtrl = _effectCtrl;
    }

    private void OnTriggerEnter(Collider _other)
    {
        if (_other.CompareTag("PowerCore"))
        {
            effectCtrl.EffectOn(3);
            ArrayCurrencyCommand.Use(ECurrencyCommand.COLLECT_CORE, coreAmount);
            Destroy(_other.gameObject);
        }
    }

    [SerializeField]
    private uint coreAmount = 20;

    private EffectController effectCtrl = null;
}
