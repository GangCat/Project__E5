using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EffectManager : MonoBehaviour
{
    public void Init()
    {
        // ΩÃ±€≈Ê ∆–≈œ √ ±‚»≠
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        foreach (EffectPlayerBase EPB in GetComponentsInChildren<EffectPlayerBase>())
            EPB.Init();
    }
    
    public void PlayAttackEffect(EObjectType _objectType)
    {
        Vector3 effectPosition = DetermineEffectPosition(_objectType);
        
        switch (_objectType)
        {
            case EObjectType.UNIT_HERO:
                EffectPlayer_Attack.instance.PlayEffect(EffectPlayer_Attack.AttackEffectType.HERO, effectPosition);
                break;
            case EObjectType.UNIT_01:
                EffectPlayer_Attack.instance.PlayEffect(EffectPlayer_Attack.AttackEffectType.UNIT_01, effectPosition);
                break;
            case EObjectType.UNIT_02:
                EffectPlayer_Attack.instance.PlayEffect(EffectPlayer_Attack.AttackEffectType.UNIT_02, effectPosition);
                break;
            case EObjectType.ENEMY_UNIT:
                EffectPlayer_Attack.instance.PlayEffect(EffectPlayer_Attack.AttackEffectType.ENEMY_01, effectPosition);
                break;
            default:
                Debug.LogWarning("Unhandled object type: " + _objectType);
                break;
        }
    }
    
    private Vector3 DetermineEffectPosition(EObjectType _objectType)
    {
        if (friendlyObject == null) return Vector3.zero;
        if (enemyObject == null) return Vector3.zero;
        
        switch (_objectType)
        {
            case EObjectType.UNIT_HERO:
            case EObjectType.UNIT_01:
            case EObjectType.UNIT_02:
                if(friendlyObject != null)
                    return friendlyObject.GetEffectPosition();
                break;
            case EObjectType.ENEMY_UNIT:
                if(enemyObject != null)
                    return enemyObject.GetEffectPosition();
                break;
            default:
                break;
        }
        Debug.LogWarning("Effect position not found for type: " + _objectType);
        return Vector3.zero;
    }
    
    public static EffectManager instance;
    private FriendlyObject friendlyObject;
    private EnemyObject enemyObject;
    
}
