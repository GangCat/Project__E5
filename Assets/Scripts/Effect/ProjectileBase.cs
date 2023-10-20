using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBase : MonoBehaviour
{
    public virtual void Init(Vector3 _targetPos)
    {
        moveDir = (_targetPos - transform.position).normalized;
        transform.rotation = Quaternion.LookRotation(moveDir);
        StartCoroutine("AutoDestroyCoroutine");
    }

    protected IEnumerator AutoDestroyCoroutine()
    {
        float startTime = Time.time;
        while(Time.time - startTime < autoDestroyTime)
        {
            transform.position += moveDir * moveSpeed * Time.deltaTime;
            yield return null;
        }

        DestroySelf();
    }

    protected void DestroySelf()
    {
        Destroy(gameObject);
    }

    [SerializeField]
    protected float moveSpeed = 0f;
    [SerializeField]
    protected float autoDestroyTime = 0f;

    protected Vector3 moveDir = Vector3.zero;
}
