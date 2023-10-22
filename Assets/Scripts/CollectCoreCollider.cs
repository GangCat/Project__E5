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
            AudioManager.instance.PlayAudio_Misc(EAudioType_Misc.PICKUP);
            
            effectCtrl.EffectOn(2, _other.transform.position);
            ArrayCurrencyCommand.Use(ECurrencyCommand.COLLECT_CORE, coreAmount);
            Destroy(_other.gameObject);
        }
    }

    [SerializeField]
    private uint coreAmount = 20;

    private EffectController effectCtrl = null;
    
}
