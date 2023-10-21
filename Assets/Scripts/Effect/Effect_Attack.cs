using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect_Attack : MonoBehaviour
{
    private void OnEnable()
    {
        if(effectPrefab)
        {
            if (effectPrefab == null)
            {
                instance = Instantiate(effectPrefab, transform.position, Quaternion.identity);
                instance.transform.SetParent(transform);
            }
            else
            {
                instance.SetActive(true);
            }
        }
    }

    private void OnDisable()
    {
        if(instance)
        {
            instance.SetActive(false);
        }
    }
    
    private GameObject instance;
    
    [SerializeField] private GameObject effectPrefab;
}
