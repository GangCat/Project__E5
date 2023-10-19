using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StructureCollider : MonoBehaviour, IGetObjectType, IDamageable
{
    public void Init(VoidFloatDelegate _getDmgCallback, EObjectType _objectType)
    {
        getDmgCallback = _getDmgCallback;
        objectType = _objectType;
        effectCtrl = GetComponent<EffectController>();
    }

    public EffectController GetEffectCtrl => effectCtrl;

    public void Init()
    {
        beamRenderer = GetComponentInChildren<MeshRenderer>();
    }

    public void GetDmg(float _dmg)
    {
        effectCtrl.EffectOn(2);
        getDmgCallback?.Invoke(_dmg);
    }

    public EObjectType GetObjectType()
    {
        return objectType;
    }

    public void ShowHBeam()
    {
        beamRenderer.enabled = true;
    }
    
    public void HideHBeam()
    {
        beamRenderer.enabled = false;
    }

    private VoidFloatDelegate getDmgCallback = null;
    private EObjectType objectType = EObjectType.NONE;
    private Renderer beamRenderer = null;
    private EffectController effectCtrl = null;
}
