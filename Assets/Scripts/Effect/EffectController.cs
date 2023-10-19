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
            instance[i].SetActive(false); // ó������ ��Ȱ��ȭ
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
    
    [SerializeField] private Transform[] effectPositions; // ����Ʈ�� �ߵ��� ��ġ��
    [SerializeField] private GameObject effectPrefab; // ��ƼŬ ����Ʈ ������

    private GameObject[] instance; // �ν��Ͻ�ȭ�� ����Ʈ��
    
}