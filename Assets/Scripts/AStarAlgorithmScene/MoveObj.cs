using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoveObj : MonoBehaviour
{
    public Transform targetPos;
    public Transform attackTargetPos;
    public FollowObjText followText;
    public bool isAttack;
    public bool wantAttack;
    public bool isDrawPath;
    public bool isFriendly;
    public float moveSpeed;

    public GameObject myQueuePrefab;
    GameObject queuePrefab;
    public QueueImage queueImage;
    public QueueImage attackQueueImage;

    int count = 0;

    public virtual void RequestPath()
    {
        if (isFriendly)
            PF_PathRequestManager.FriendlyRequestPath(transform.position, targetPos.position, OnPathFound, isAttack);
        else
            PF_PathRequestManager.EnemyRequestPath(transform.position, targetPos.position, OnPathFound, isAttack);

        if (isAttack)
            queuePrefab = Instantiate(myQueuePrefab, attackQueueImage.transform);
        else
            queuePrefab = Instantiate(myQueuePrefab, queueImage.transform);

        ++count;


        if (followText)
            followText.UpdateText("Wait");
    }

    protected void OnPathFound(PF_Node[] _newPath, bool _pathSuccessful, PF_Node _newTargetNode = null)
    {
        if (_pathSuccessful)
        {
            arrPath = _newPath;
            targetIdx = 0;
            if (arrPath.Length > 0)
                curWayNode = arrPath[targetIdx];
        }

        if (followText)
            followText.UpdateText("Move");

        if (isAttack)
            attackQueueImage.Deqeueu();
        else
            queueImage.Deqeueu();

        if (count > 1)
            isAttack = true;

        StartCoroutine(MoveCoroutine());
    }

    private void Update()
    {
        if (isAttack)
            GetComponent<MeshRenderer>().material.color = Color.yellow;
    }

    protected IEnumerator MoveCoroutine()
    {
        curWayNode = arrPath[targetIdx];
        Vector3 moveDir;
        for(int i = 0; i < arrPath.Length; ++i)
        {
            moveDir = (curWayNode.worldPos - transform.position).normalized;
            while (Vector3.Distance(transform.position, curWayNode.worldPos) > 0.1f)
            {
                transform.position += moveDir * Time.deltaTime * moveSpeed;

                if(wantAttack)
                {
                    if(Vector3.Distance(targetPos.position, transform.position) < 5f && isAttack == false)
                    {
                        // 공격 우선처리 수정 전에는 이 밑에 줄 주석처리하면 됨.
                        // 수정 후는 주석 해제
                        isAttack = true;
                        RequestPath();
                        yield break;
                    }
                }

                yield return null;
            }
            ++targetIdx;
            if (targetIdx >= arrPath.Length)
                yield break;

            curWayNode = arrPath[targetIdx];
        }
    }

    public void OnDrawGizmos()
    {
        if (arrPath != null && isDrawPath)
        {
            for (int i = targetIdx; i < arrPath.Length; ++i)
            {
                Gizmos.color = Color.black;
                Gizmos.DrawCube(arrPath[i].worldPos, Vector3.one * 0.4f);

                if (i == targetIdx)
                    Gizmos.DrawLine(transform.position, arrPath[i].worldPos);
                else
                    Gizmos.DrawLine(arrPath[i - 1].worldPos, arrPath[i].worldPos);
            }
        }
    }

    protected int targetIdx = 0;
    protected PF_Node[] arrPath = null;
    protected PF_Node curWayNode = null;
    
}
