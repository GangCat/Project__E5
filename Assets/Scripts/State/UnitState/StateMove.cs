using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMove : IState
{
    public void Start(ref SUnitState _structState)
    {
        myTr = _structState.myTr;
        moveSpeed = _structState.moveSpeed;
    }

    public void Update(ref SUnitState _structState)
    {
        if (_structState.isWaitForNewPath) return;
        if (_structState.isPause) return;


        myTr.rotation = Quaternion.LookRotation(_structState.targetPos - myTr.position);

        Vector3 moveVec = _structState.targetPos - myTr.position;
        myTr.position += moveVec.normalized * moveSpeed * Time.deltaTime;
    }

    public void End(ref SUnitState _structState)
    {
    }

    private Transform myTr = null;
    private float moveSpeed = 0f;
}