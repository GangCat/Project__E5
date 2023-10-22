using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// 최근꺼

public class SelectableObject : MonoBehaviour, IDamageable, IGetObjectType, IPauseObserver
{
    protected enum EMoveState { NONE = -1, NORMAL, ATTACK, PATROL, CHASE, FOLLOW, FOLLOW_ENEMY }
    public virtual void Init()
    {
        gridInstance = PF_Grid.instance;
        ArrayPauseCommand.Use(EPauseCommand.REGIST, this);
        SelectableObjectManager.InitNodeEnemy(transform.position, out nodeIdx);

        stateMachine = GetComponent<StateMachine>();
        statusHp = GetComponent<StatusHp>();
        // 이거 메모리풀이라서 애들 비활성화될 때 unselect당하고 비활성화해서 다시 활성화할 때 못찾아서 에러나는거임.
        if (!displayCircleObject)
            displayCircleObject = GetComponentInChildren<PickObjectDisplay>();
        displayCircleObject.Init();
        statusHp.Init();

        if (stateMachine)
        {
            effectCtrl = GetComponent<EffectController>();
            effectCtrl.Init();
            stateMachine.Init(GetCurState, effectCtrl);
            ResetStateStack();
            StateIdle();
            UpdateCurNode();
        }

    }

    public string GetObjectName => objectDisplayName;
    public string GetObjectDescription => objectDisplayDescription;
    public Vector3 GetPos => transform.position;
    public int MaxHp => statusHp.MaxHp;
    public float AttRange => attackRange;
    public float GetCurHpPercent => statusHp.GetCurHpPercent;
    public bool IsTempSelect { get; set; }
    public float AttDmg
    {
        get
        {
            if (stateMachine)
                return stateMachine.AttDmg;
            else
                return 0;
        }
    }
    public float AttRate
    {
        get
        {
            if (stateMachine)
                return stateMachine.AttRate;
            else
                return 0;
        }
    }

    public virtual void Select(int _listIdx = 0)
    {
        isSelect = true;
        ActivateCircle();
    }

    public virtual void unSelect()
    {
        isSelect = false;
        DeActivateCircle();
    }

    public void ActivateCircle()
    {
        displayCircleObject.SetActive(true);
        //if(displayCircleObject == null)
        //    displayCircleObject = Instantiate(selectDisplayCircle, transform.position, Quaternion.identity, transform);
    }

    public void DeActivateCircle()
    {
        displayCircleObject.SetActive(false);
        //if (displayCircleObject != null)
        //    Destroy(displayCircleObject);
    }

    public EObjectType GetObjectType()
    {
        return objectType;
    }

    public Vector3 Position
    {
        get => transform.position;
        set => transform.position = value;
    }

    public void Rotate(float _angle)
    {
        Functions.RotateYaw(transform, _angle);
    }
    public virtual void UpdateCurNode()
    {

    }

    public virtual void GetDmg(float _dmg)
    {
    }

    public virtual void MoveAttack(Vector3 _targetPos)
    {
        targetPos = _targetPos;
        curMoveCondition = EMoveState.ATTACK;
        ResetStateStack();
        PushState();
        StateMove();
    }

    public virtual void Stop()
    {
        ResetStateStack();
        PushState();
        StateStop();
    }

    #region StateIdleCondition
    protected void StateIdle()
    {
        stateMachine.TargetTr = null;
        targetTr = null;
        StopAllCoroutines();
        UpdateCurNode();
        ChangeState(EState.IDLE);
        StartCoroutine("CheckIsEnemyInChaseStartRangeCoroutine");
    }
    #endregion

    protected Collider[] arrtempcol = null;

    protected virtual IEnumerator CheckIsEnemyInChaseStartRangeCoroutine()
    {
        yield return new WaitForSeconds(0.01f);
        while (true)
        {
            // 추적 범위만큼 overlapLayerMask에 해당하는 충돌체를 overlapSphere로 검사
            //Collider[] arrCollider = null;
            arrtempcol = OverlapSphereForDetectTarget(chaseStartRange);
            // 충돌한 오브젝트가 존재한다면
            if (arrtempcol.Length > 0)
            {
                foreach (Collider c in arrtempcol)
                {
                    // 해당 오브젝트의 ObjectType을 가져온다.
                    EObjectType targetType = c.GetComponent<IGetObjectType>().GetObjectType();
                    // 쫓는 대상은 없는데 검사한 대상이 적 유닛이 아닐 경우(적 유닛만 사용할 조건이기 때문)
                    // 큰 적도 있으니까 그것도 추가하기
                    if (!targetType.Equals(EObjectType.ENEMY_UNIT))
                    {
                        stateMachine.TargetTr = c.transform;
                        targetTr = c.transform;
                        prevMoveCondition = curMoveCondition;
                        curMoveCondition = EMoveState.CHASE;
                        PushState();
                        StateMove();
                        yield break;
                    }
                }
            }
            yield return new WaitForSeconds(0.05f);
        }
    }

    protected IEnumerator CheckIsTargetInChaseFinishRangeCoroutine()
    {
        Collider[] arrCol = null;
        yield return null;
        while (true)
        {
            if (targetTr == null)
            {
                stateMachine.TargetTr = null;
                FinishState();
                //StateMove();
                yield break;
            }
            else if (!targetTr.gameObject.activeSelf)
            {
                stateMachine.TargetTr = null;
                targetTr = null;
                FinishState();
                //StateMove();
                yield break;
            }
            else if (!targetTr.Equals(stateMachine.TargetTr))
            {
                stateMachine.TargetTr = null;
                targetTr = null;
                FinishState();
                //StateMove();
                yield break;
            }
            else if (!IsTargetInRangeFromMyPos(targetTr.position, chaseEndRange))
            {
                stateMachine.TargetTr = null;
                targetTr = null;
                FinishState();
                //StateMove();
                yield break;
            }
            if (!targetTr.GetComponent<StructureWall>())
            {
                arrCol = OverlapSphereForDetectTarget(chaseStartRange);
                if (arrCol.Length > 0)
                {
                    foreach (Collider c in arrCol)
                    {
                        // 해당 오브젝트의 ObjectType을 가져온다.
                        EObjectType targetType = c.GetComponent<IGetObjectType>().GetObjectType();
                        if (targetType.Equals(EObjectType.WALL))
                        {
                            stateMachine.TargetTr = c.transform;
                            targetTr = c.transform;
                            yield break;
                        }
                    }
                }
            }
            yield return new WaitForSeconds(0.5f);
        }
    }

    protected void CheckIsTargetInAttackRange()
    {
        if (targetTr && targetTr.gameObject.activeSelf)
        {
            if (IsTargetInRangeFromMyPos(targetTr.position, attackRange))
                StateAttack();
        }
    }

    protected IEnumerator CheckIsTargetInAttackRangeCoroutine()
    {
        while (true)
        {
            if (targetTr != null && targetTr.gameObject.activeSelf.Equals(true))
            {
                if (IsTargetInRangeFromMyPos(targetTr.position, attackRange))
                    StateAttack();
            }
            yield return new WaitForSeconds(0.1f);
        }
    }

    #region StateMoveConditions
    protected virtual void StateMove()
    {
        StopAllCoroutines();
        curWayNode = null;
        stateMachine.SetWaitForNewPath(true);
        UpdateCurNode();
        ChangeState(EState.MOVE);

        if (gameObject.activeSelf)

        {
            switch (curMoveCondition)
            {
                case EMoveState.ATTACK:
                    StartCoroutine("CheckNormalMoveCoroutine");

                    break;
                case EMoveState.CHASE:
                    if (!targetTr || targetTr.gameObject.activeSelf.Equals(false))
                    {
                        if (prevMoveCondition != EMoveState.NONE)
                        {
                            curMoveCondition = prevMoveCondition;
                            prevMoveCondition = EMoveState.NONE;
                            StateMove();
                        }
                        else
                        {
                            ResetStateStack();
                            PushState();
                            StateStop();
                        }
                    }
                    else
                    {
                        StartCoroutine("CheckFollowMoveCoroutine");
                        StartCoroutine("CheckIsTargetInChaseFinishRangeCoroutine");
                        //StartCoroutine("CheckIsTargetInAttackRangeCoroutine");
                    }
                    break;
                default:
                    break;
            }
        }
    }

    protected virtual IEnumerator CheckNormalMoveCoroutine()
    {
        RequestPath(transform.position, targetPos);

        while (curWayNode == null)
            yield return new WaitForSeconds(0.05f);

        stateMachine.SetWaitForNewPath(false);
        StartCoroutine("CheckIsEnemyInChaseStartRangeCoroutine");
        while (true)
        {
            if (!hasTargetNode)
            {
                if (IsObjectBlocked())
                {
                    curWayNode = null;
                    stateMachine.SetWaitForNewPath(true);
                    RequestPath(transform.position, targetPos);

                    while (curWayNode == null)
                        yield return new WaitForSeconds(0.05f);

                    stateMachine.SetWaitForNewPath(false);

                    //    stateMachine.SetWaitForNewPath(true);
                    //    curWayNode = GetNearWalkableNode(curWayNode);
                    //    yield return new WaitForSeconds(0.1f);
                    //    hasTargetNode = true;
                    //    stateMachine.SetWaitForNewPath(false);
                    //}
                }

                if (!curWayNode.walkable)
                {
                    //curWayNode = null;
                    //stateMachine.SetWaitForNewPath(true);
                    //RequestPath(transform.position, targetPos);

                    //while (curWayNode == null)
                    //    yield return new WaitForSeconds(0.05f);

                    //stateMachine.SetWaitForNewPath(false);
                    //Debug.Log("!curWayNode.walkable");

                    stateMachine.SetWaitForNewPath(true);
                    curWayNode = GetNearWalkableNode(curWayNode);
                    yield return new WaitForSeconds(0.1f);
                    stateMachine.SetWaitForNewPath(false);
                }

                // 노드에 도착할 때마다 새로운 노드로 이동 갱신
                if (IsTargetInRangeFromMyPos(stateMachine.TargetPos, 0.1f))
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

                //yield return new WaitForEndOfFrame();
                yield return null;
            }
        }
    }

    protected virtual IEnumerator CheckFollowMoveCoroutine()
    {
        RequestPath(transform.position, targetTr.position);

        while (curWayNode == null)
            yield return new WaitForSeconds(0.05f);

        float elapsedTime = 0f;
        stateMachine.SetWaitForNewPath(false);

        while (true)
        {
            if (!targetTr)
            {
                stateMachine.TargetTr = null;
                FinishState();
                yield break;
            }
            else if (targetTr.gameObject.activeSelf.Equals(false))
            {
                targetTr = null;
                stateMachine.TargetTr = null;
                FinishState();
                yield break;
            }
            elapsedTime += Time.deltaTime;

            if (elapsedTime > stopDelay)
            {
                elapsedTime = 0f;
                if (Vector3.SqrMagnitude(transform.position - targetTr.position) > Mathf.Pow(followOffset, 2f))
                {
                    curWayNode = null;
                    RequestPath(transform.position, targetTr.position);
                    stateMachine.SetWaitForNewPath(true);

                    while (curWayNode == null)
                        yield return new WaitForSeconds(0.05f);

                    stateMachine.SetWaitForNewPath(false);
                }
            }
            else
            {
                //while (curWayNode == null)
                //    yield return new WaitForSeconds(0.05f);

                if (IsObjectBlocked())
                {
                    stateMachine.SetWaitForNewPath(true);
                    curWayNode = GetNearWalkableNode(curWayNode);
                    yield return new WaitForSeconds(0.1f);
                    stateMachine.SetWaitForNewPath(false);
                }

                if (!curWayNode.walkable)
                {
                    curWayNode = null;
                    RequestPath(transform.position, targetTr.position);
                    stateMachine.SetWaitForNewPath(true);

                    while (curWayNode == null)
                        yield return new WaitForSeconds(0.05f);

                    stateMachine.SetWaitForNewPath(false);

                    //stateMachine.SetWaitForNewPath(true);
                    //curWayNode = GetNearWalkableNode(curWayNode);
                    //yield return null;
                    //stateMachine.SetWaitForNewPath(false);
                }

                if (IsTargetInRangeFromMyPos(curWayNode.worldPos, 0.1f))
                {
                    hasTargetNode = false;
                    ++targetIdx;
                    UpdateCurNode();
                    CheckIsTargetInAttackRange();

                    if (targetIdx >= arrPath.Length)
                    {
                        curWayNode = null;
                        stateMachine.SetWaitForNewPath(true);
                    }
                    else
                        UpdateTargetPos();
                }
            }
            yield return null;
        }
    }

    protected void OnPathFound(PF_Node[] _newPath, bool _pathSuccessful, PF_Node _newTargetNode = null)
    {
        if (_pathSuccessful)
        {
            if (_newTargetNode != null)
                targetPos = _newTargetNode.worldPos;
            arrPath = _newPath;
            targetIdx = 0;
            if (arrPath.Length > 0)
                UpdateTargetPos();
        }
        else
        {
            StopAllCoroutines();
            stateMachine.TargetTr = null;
            targetTr = null;
            curWayNode = null;
            stateMachine.SetWaitForNewPath(true);
            Invoke("StateMove", 2f);
        }
    }

    protected void UpdateTargetPos()
    {
        curWayNode = arrPath[targetIdx];
        stateMachine.TargetPos = curWayNode.worldPos;
    }

    protected virtual bool IsObjectBlocked()
    {
        if (curWayNode == null) return false;

        curPos = transform.position;
        if (Physics.Linecast(curPos, curWayNode.worldPos, blockCheckLayerMask))
            return true;

        return false;
    }
    #endregion

    #region StateStopCondition
    protected void StateStop()
    {
        StopAllCoroutines();
        ChangeState(EState.STOP);
        UpdateCurNode();
        StartCoroutine("CheckStopCoroutine");
    }

    protected IEnumerator CheckStopCoroutine()
    {
        yield return new WaitForSeconds(stopDelay);

        FinishState();
    }
    #endregion

    #region StateAttackCondition
    protected void StateAttack()
    {
        StopAllCoroutines();
        PushState();
        if (objectType.Equals(EObjectType.TURRET))
            ChangeState(EState.TURRET_ATTACK);
        else
            ChangeState(EState.ATTACK);
        UpdateCurNode();
        StartCoroutine("AttackCoroutine");
    }

    protected IEnumerator AttackCoroutine()
    {
        yield return null;
        targetTr = stateMachine.TargetTr;
        while (true)
        {
            if (targetTr == null)
            {
                stateMachine.TargetTr = null;
                FinishState();
                yield break;
            }
            else if (targetTr.gameObject.activeSelf == false)
            {
                // 추격, 정찰, 대기, 홀드 등 뭐든간에 이전으로 돌아감.
                targetTr = null;
                stateMachine.TargetTr = null;
                FinishState();
                yield break;
            }
            else if (targetTr != stateMachine.TargetTr)
            {
                FinishState();
                yield break;
            }
            else if (!IsTargetInRangeFromMyPos(targetTr.position, attackRange))
            {
                FinishState();
                yield break;
            }

            yield return null;
        }
    }
    #endregion

    protected void FinishState()
    {
        StopAllCoroutines();
        stateMachine.FinishState();
    }

    protected void ResetStateStack()
    {
        StopAllCoroutines();
        targetTr = null;
        stateMachine.TargetTr = null;
        stateMachine.ResetState();
    }

    protected void PushState()
    {
        stateMachine.PushCurState();
    }

    protected void ChangeState(EState _state)
    {
        stateMachine.ChangeState(_state);
    }

    protected bool IsTargetInRangeFromMyPos(Vector3 _targetPos, float _range)
    {
        return Vector3.SqrMagnitude(transform.position - _targetPos) < Mathf.Pow(_range, 2);
    }

    protected Collider[] OverlapSphereForDetectTarget(float _range)
    {
        return Physics.OverlapSphere(transform.position, _range, overlapLayerMask);
    }

    protected virtual void GetCurState(EState _curStateEnum)
    {
        switch (_curStateEnum)
        {
            case EState.IDLE:
                StateIdle();
                break;
            case EState.MOVE:
                StateMove();
                break;
            case EState.STOP:
                StateStop();
                break;
            case EState.ATTACK:
                StateAttack();
                break;
            default:
                break;
        }
    }

    protected PF_Node GetNearWalkableNode(PF_Node _curWayNode)
    {
        return gridInstance.GetAccessibleNodeWithoutTargetNode(_curWayNode);
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

        if (isDrawRange)
        {// 공격 범위를 그리는 기즈모 (빨간색 원)
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackRange);

            // 추적 시작 범위를 그리는 기즈모 (노란색 원)
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, chaseStartRange);

            // 추적 종료 범위를 그리는 기즈모 (초록색 원)
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, chaseEndRange);
        }
    }

    protected virtual void RequestPath(Vector3 _startPos, Vector3 _endPos)
    {
        PF_PathRequestManager.FriendlyRequestPath(_startPos, _endPos, OnPathFound);
    }

    public void CheckPause(bool _isPause)
    {
        if (stateMachine != null)
            stateMachine.SetIsPause(_isPause);
    }

    protected void OnDisable()
    {
        StopAllCoroutines();
    }

    [Header("-Unit Attribute")]
    [SerializeField]
    protected EObjectType objectType = EObjectType.NONE;
    [SerializeField]
    protected GameObject selectDisplayCircle = null;
    [SerializeField]
    protected string objectDisplayName = null;
    [TextArea]
    [SerializeField]
    protected string objectDisplayDescription = null;
    [SerializeField]
    protected bool isDrawRange = false;
    [SerializeField]
    protected bool isDrawPath = false;


    [Header("-Unit Control Values")]
    [SerializeField]
    protected float chaseStartRange = 0f;
    [SerializeField]
    protected float chaseEndRange = 0f;
    [SerializeField]
    protected float attackRange = 0f;
    [SerializeField]
    protected float stopDelay = 2f;
    [SerializeField]
    protected float followOffset = 3f;
    [SerializeField]
    protected LayerMask overlapLayerMask;
    [SerializeField]
    protected LayerMask blockCheckLayerMask;

    protected EMoveState curMoveCondition = EMoveState.NONE;
    protected EMoveState prevMoveCondition = EMoveState.NONE;
    protected StateMachine stateMachine = null;

    protected Vector3 targetPos = Vector3.zero;
    protected Vector3 curPos = Vector3.zero;

    protected Transform targetTr = null;

    protected int targetIdx = 0;
    protected PF_Node[] arrPath = null;
    protected PF_Node curWayNode = null;

    protected StatusHp statusHp = null;

    protected int nodeIdx = 0;
    protected PickObjectDisplay displayCircleObject = null;

    protected PF_Grid gridInstance = null;

    protected bool hasTargetNode = false;

    protected EffectController effectCtrl = null;

    protected bool isSelect = false;
}