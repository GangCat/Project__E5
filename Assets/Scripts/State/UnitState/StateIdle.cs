using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateIdle : IState
{
    public void Start(ref SUnitState _structState)
    {
        anim = _structState.animator;
    }

    public void Update(ref SUnitState _structState)
    {
        // 일시정지 상태 처리
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
    }

    public void End(ref SUnitState _structState)
    {
    }

    private Animator anim = null;
}
