using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMove : IState
{
    public void Start(ref SUnitState _structState)
    {
        myTr = _structState.myTr;
        moveSpeed = _structState.moveSpeed;
        anim = _structState.animator;
        if (anim)
            anim.SetBool("isMove", true);
    }

    public void Update(ref SUnitState _structState)
    {
        if (_structState.isWaitForNewPath) return;
        if (_structState.isPause)
        {
            if (anim)
                anim.StartPlayback();
            return;
        }
        else
        {
            if (anim)
                anim.StopPlayback();
        }


        myTr.rotation = Quaternion.LookRotation(_structState.targetPos - myTr.position);

        Vector3 moveVec = _structState.targetPos - myTr.position;
        myTr.position += moveVec.normalized * moveSpeed * Time.deltaTime;
    }

    public void End(ref SUnitState _structState)
    {
        if (anim)
            anim.SetBool("isMove", false);
    }

    private Transform myTr = null;
    private float moveSpeed = 0f;
    private Animator anim = null;
}