using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectController : MonoBehaviour
{
    public void Init()
    {
        instance = new GameObject[effectPositions.Length];
        for (int i = 0; i < effectPositions.Length; i++)
        {
            instance[i] = Instantiate(effectPrefab, effectPositions[i].position, Quaternion.identity, effectPositions[i]);
            instance[i].SetActive(false); // 처음에는 비활성화
        }
    }

    public void PlayEffects()
    {
        foreach (var effect in instance)
        {
            effect.SetActive(true);
        }
    }

    public void StopEffects()
    {
        foreach (var effect in instance)
        {
            effect.SetActive(false);
        }
    }
    
    [SerializeField] private Transform[] effectPositions; // 이펙트가 발동될 위치들
    [SerializeField] private GameObject effectPrefab; // 파티클 이펙트 프리팹

    private GameObject[] instance; // 인스턴스화된 이펙트들
    
}