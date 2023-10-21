using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyObject : SelectableObject
{
    [System.Serializable]
    public enum EEnemySpawnType { NONE = -1, WAVE_SPAWN, MAP_SPAWN, LENGTH }

    public void Init(EEnemySpawnType _spawnType, int _myIdx, Vector3 _dest)
    {
        spawnType = _spawnType;
        myIdx = _myIdx;
        dest = _dest;
        gameObject.layer = LayerMask.NameToLayer("EnemySelectableObject");
    }

    public override void UpdateCurNode()
    {
        SelectableObjectManager.UpdateEnemyNodeWalkable(transform.position, nodeIdx);
    }

    public override void GetDmg(float _dmg)
    {
        if (statusHp.DecreaseHpAndCheckIsDead(_dmg))
        {
            StopAllCoroutines();
            DeactivateUnit();
        }
        else
            effectCtrl.EffectOn(0);
        //else if (isSelect)
        //{
        //    SelectableObjectManager.UpdateHp(listIdx);
        //}
    }

    protected override IEnumerator CheckNormalMoveCoroutine()
    {
        RequestPath(transform.position, targetPos);

        while (curWayNode == null)
            yield return new WaitForSeconds(0.05f);

        stateMachine.SetWaitForNewPath(false);
        StartCoroutine("CheckIsEnemyInChaseStartRangeCoroutine");
        while (true)
        {
            //if (!hasTargetNode)
            //{
            //    if (IsObjectBlocked())
            //    {
            //        stateMachine.SetWaitForNewPath(true);
            //        curWayNode = GetNearWalkableNode(curWayNode);
            //        yield return new WaitForSeconds(0.1f);
            //        hasTargetNode = true;
            //        stateMachine.SetWaitForNewPath(false);
            //        Debug.Log("IsObjectBlocked");
            //    }
            //}

            //if (!curWayNode.walkable)
            //{
            //    //curWayNode = null;
            //    //stateMachine.SetWaitForNewPath(true);
            //    //RequestPath(transform.position, targetPos);

            //    //while (curWayNode == null)
            //    //    yield return new WaitForSeconds(0.05f);

            //    //stateMachine.SetWaitForNewPath(false);
            //    Debug.Log("!curWayNode.walkable");
            //    stateMachine.SetWaitForNewPath(true);
            //    curWayNode = GetNearWalkableNode(curWayNode);
            //    yield return new WaitForSeconds(0.1f);
            //    stateMachine.SetWaitForNewPath(false);
            //}

            // 노드에 도착할 때마다 새로운 노드로 이동 갱신
            if (isTargetInRangeFromMyPos(stateMachine.TargetPos, 0.1f))
            {
                hasTargetNode = false;
                ++targetIdx;
                UpdateCurNode();
                // 목적지에 도착시 
                CheckIsTargetInAttackRange();

                if (targetIdx >= arrPath.Length)
                {
                    curWayNode = null;

                    if (Vector3.SqrMagnitude(targetPos - transform.position) > Mathf.Pow(1.4f, 2f))
                    {
                        if (IsDestinationClose())
                            targetPos = dest;

                        RequestPath(transform.position, targetPos);

                        stateMachine.SetWaitForNewPath(true);
                        while (curWayNode == null)
                            yield return new WaitForSeconds(0.05f);

                        stateMachine.SetWaitForNewPath(false);
                        continue;
                    }

                    FinishState();
                    stateMachine.SetWaitForNewPath(false);
                    yield break;
                }
                UpdateTargetPos();
            }

            yield return new WaitForEndOfFrame();
        }
    }

    private bool IsDestinationClose()
    {
        return Vector3.SqrMagnitude(dest - transform.position) < 10000;
    }

    private void DeactivateUnit()
    {
        // Enemy Dead Audio
        AudioManager.instance.PlayAudio_Destroy(objectType);
        ArrayPauseCommand.Use(EPauseCommand.REMOVE, this);

        if (objectType.Equals(EObjectType.ENEMY_BIG))
        {
            for (int i = 0; i < 15; ++i)
            {
                Instantiate(powerCorePrefab, transform.position + Functions.GetRandomPosition(3, 0), Quaternion.identity);
                Destroy(gameObject);
            }
        }
        else
        {
            SelectableObjectManager.ResetEnemyNodeWalkable(transform.position, nodeIdx);

            ArrayEnemyObjectCommand.Use((EEnemyObjectCommand)spawnType, gameObject, myIdx);
            if (Random.Range(0.0f, 100.0f) < 30f)
                Instantiate(powerCorePrefab, transform.position, Quaternion.identity);

        }
    }

    protected override void RequestPath(Vector3 _startPos, Vector3 _endPos)
    {
        PF_PathRequestManager.EnemyRequestPath(_startPos, _endPos, OnPathFound);
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }

    public Vector3 GetEffectPosition()
    {
        Vector3 offset = new Vector3(0, 1, 0);
        return transform.position + offset;
    }
    
    
    [SerializeField]
    private GameObject powerCorePrefab = null;

    private EEnemySpawnType spawnType = EEnemySpawnType.NONE;
    private int myIdx = 0;
    private Vector3 dest = Vector3.zero;
}
