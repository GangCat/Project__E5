using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileEnemyBig : ProjectileBase
{
    public override void Init(Vector3 _targetPos)
    {
        moveDir = (_targetPos - transform.position).normalized;
        transform.rotation = Quaternion.LookRotation(moveDir);
        StartCoroutine("ChargeToAttackCoroutine");
    }

    private IEnumerator ChargeToAttackCoroutine()
    {
        while(transform.position.y >= 0)
        {
            transform.position += moveDir * moveSpeed * Time.deltaTime;
            yield return null;
            moveSpeed += Time.deltaTime;
        }

        Collider[] arrCol = Physics.OverlapSphere(transform.position, overlapRadius, targetMask);

        for(int i = 0; i < arrCol.Length; ++i)
        {
            arrCol[i].GetComponent<FriendlyObject>().GetDmg(attDmg);
        }

        Instantiate(impactGo, transform.position, Quaternion.Euler(new Vector3(-90f, 0f, 0f)));
        Destroy(gameObject);
    }

    [SerializeField]
    private float overlapRadius = 0f;
    [SerializeField]
    private LayerMask targetMask;
    [SerializeField]
    private float attDmg = 0f;
    [SerializeField]
    private GameObject impactGo = null;
}
