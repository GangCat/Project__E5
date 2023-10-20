using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine : MonoBehaviour
{
    public delegate void CurStateEnumDelegate(EState _curEState);

    public void Init(CurStateEnumDelegate _curStateEnumCallback, EffectController _effectCtrl)
    {
        curStateEnumCallback = _curStateEnumCallback;
        unitStateInfo.effectCtrl = _effectCtrl;

        IState stateIdle = new StateIdle();
        IState stateMove = new StateMove();
        IState stateStop = new StateStop();
        IState stateHold = new StateHold();
        IState stateAttack = new StateAttack();
        IState stateTurretAttack = new StateTurretAttack();

        arrState = new IState[(int)EState.LENGTH];

        arrState[(int)EState.IDLE] = stateIdle;
        arrState[(int)EState.MOVE] = stateMove;
        arrState[(int)EState.STOP] = stateStop;
        arrState[(int)EState.HOLD] = stateHold;
        arrState[(int)EState.ATTACK] = stateAttack;
        arrState[(int)EState.TURRET_ATTACK] = stateTurretAttack;
        

        unitStateInfo.myTr = transform;
        oriDmg = unitStateInfo.attDmg;

        curState = stateIdle;
        curStateEnum = EState.IDLE;
        stackStateEnum.Push(curStateEnum);
    }

    public float AttRate => unitStateInfo.attRate;
    public float AttDmg => unitStateInfo.attDmg;

    public void SetIsPause(bool _isPause)
    {
        unitStateInfo.isPause = _isPause;
    }

    public void SetMyTr(Transform _myTr)
    {
        unitStateInfo.myTr = _myTr;
    }

    public Vector3 TargetPos
    {
        get => unitStateInfo.targetPos;
        set => unitStateInfo.targetPos = value;
    }

    public Transform TargetTr
    {
        get => unitStateInfo.targetTr;
        set => unitStateInfo.targetTr = value;
    }

    public void UpgradeAttDmg(float _increaseDmg)
    {
        oriDmg += _increaseDmg;
        unitStateInfo.attDmg += _increaseDmg;
    }

    public void SetAttackDmg(float _ratio)
    {
        unitStateInfo.attDmg += oriDmg * _ratio;
    }

    public void ResetAttackDmg()
    {
        unitStateInfo.attDmg = oriDmg;
    }

    public void SetWaitForNewPath(bool _isWaiting)
    {
        unitStateInfo.isWaitForNewPath = _isWaiting;
    }
    public void ChangeState(EState _newState)
    {
        //curState.End(ref unitState);
        //if (curStateEnum != _newState)
        //{
        //    stackStateEnum.Push(curStateEnum);
        //    curStateEnum = _newState;
        //    curState = arrState[(int)curStateEnum];
        //}
        //curState.Start(ref unitState);

#if UNITY_EDITOR
        if (GetComponent<UnitHero>())
            HeroUnitManager.UpdateCurState(_newState);
#endif
        curState.End(ref unitStateInfo);

        //stackStateEnum.Push(curStateEnum);
        curStateEnum = _newState;
        curState = arrState[(int)curStateEnum];

        curState.Start(ref unitStateInfo);
    }

    public void FinishState()
    {
        //curState.End(ref unitState);

        //curStateEnum = stackStateEnum.Pop();
        //curState = arrState[(int)curStateEnum];
        //if (stackStateEnum.Count < 1)
        //    curStateEnumCallback?.Invoke(EState.IDLE);
        curStateEnumCallback?.Invoke(stackStateEnum.Pop());

        //curState.Start(ref unitState);
    }

    public void PushCurState()
    {
        stackStateEnum.Push(curStateEnum);
    }

    public void ResetState()
    {
        stackStateEnum.Clear();
        curStateEnum = EState.IDLE;
    }


    private void Update()
    {
        if (curState != null)
            curState.Update(ref unitStateInfo);
    }

    [SerializeField]
    private SUnitState unitStateInfo;

    private IState[] arrState = null;
    private IState curState = null;

    private Stack<EState> stackStateEnum = new Stack<EState>();
    private EState curStateEnum = EState.NONE;

    private CurStateEnumDelegate curStateEnumCallback = null;
    private float oriDmg = 0f;
}