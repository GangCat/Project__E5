using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitHero : MonoBehaviour
{
    public void Init()
    {
        myObj = GetComponent<FriendlyObject>();
        statusHp = GetComponent<StatusHp>();
        effectCtrl = GetComponent<EffectController>();
        col = GetComponentInChildren<CollectCoreCollider>();
        myObj.Init();
        col.Init(effectCtrl);
        statusHp.Init();
    }

    public void Dead()
    {
        gameObject.SetActive(false);
    }

    public void Resurrection(Vector3 _resurrectionPos)
    {
        transform.position = _resurrectionPos;
        gameObject.SetActive(true);
        myObj.SetIdleState();
        statusHp.Init();
        StartCoroutine("HealCoroutine");
        //effectCtrl.EffectOn(3);
    }

    private IEnumerator HealCoroutine()
    {
        yield return null;

        while (true)
        {
            statusHp.IncreaseCurHp(healAmount);

            yield return new WaitForSeconds(5f);
        }
    }

    [SerializeField]
    private float healAmount = 0f;

    private FriendlyObject myObj = null;
    private StatusHp statusHp = null;
    private EffectController effectCtrl = null;
    private CollectCoreCollider col = null;
}
