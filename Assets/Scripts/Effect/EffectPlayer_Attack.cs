using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class EffectPlayer_Attack : EffectPlayerBase
{


    // 필요한 경우 추가 이펙트 프리팹들을 여기에 선언

    public override void Init()  // 이펙트 프리팹 초기화
    {
        attackEffects.Add(AttackEffectType.HERO, EffectPrefab_Hero);
        attackEffects.Add(AttackEffectType.UNIT_01, EffectPrefab_Unit01);
        attackEffects.Add(AttackEffectType.UNIT_02, EffectPrefab_Unit02);
        attackEffects.Add(AttackEffectType.ENEMY_01, EffectPrefab_Enemy01);
        attackEffects.Add(AttackEffectType.ENEMY_02, EffectPrefab_Enemy02);
    }

    public void PlayEffect(AttackEffectType _effectType, Vector3 _position)
    {
        if (attackEffects.ContainsKey(_effectType))
        {
            Instantiate(attackEffects[_effectType], _position, Quaternion.identity);
        }
        else
        {
            Debug.LogWarning("The requested effect type does not exist: " + _effectType);
        }
    }


    public static EffectPlayer_Attack instance;
    
    public enum AttackEffectType { NONE = -1, HERO, UNIT_01, UNIT_02, ENEMY_01, ENEMY_02, LENGTH }

    public Dictionary<AttackEffectType, GameObject> attackEffects = new Dictionary<AttackEffectType, GameObject>();

    [SerializeField]
    private GameObject EffectPrefab_Hero;

    [SerializeField]
    private GameObject EffectPrefab_Unit01;

    [SerializeField]
    private GameObject EffectPrefab_Unit02;
    
    [SerializeField]
    private GameObject EffectPrefab_Enemy01;
    
    [SerializeField]
    private GameObject EffectPrefab_Enemy02;
}
