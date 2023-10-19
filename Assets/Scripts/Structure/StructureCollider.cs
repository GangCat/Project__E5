using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StructureCollider : MonoBehaviour, IGetObjectType, IDamageable
{
    public delegate void StructureColliderTakeDamageDelegate(float _dmg, Vector3 _pos);
    public void Init(StructureColliderTakeDamageDelegate _getDmgCallback, EObjectType _objectType)
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
        getDmgCallback?.Invoke(_dmg, transform.position);
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

    private StructureColliderTakeDamageDelegate getDmgCallback = null;
    private EObjectType objectType = EObjectType.NONE;
    private Renderer beamRenderer = null;
    private EffectController effectCtrl = null;
}
