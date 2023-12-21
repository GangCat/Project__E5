using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 상태머신의 인터페이스
/// </summary>
public interface IState
{
    /// <summary>
    /// 스테시트 시작시 1번 호출
    /// </summary>
    /// <param name="_structState"></param>
    public void Start(ref SUnitState _structState);
    /// <summary>
    /// 스테이트 업데이트시 매 프레임 호출
    /// </summary>
    /// <param name="_structState"></param>
    public void Update(ref SUnitState _structState);
    /// <summary>
    /// 스테이트 종료시 1번 호출
    /// </summary>
    /// <param name="_structState"></param>
    public void End(ref SUnitState _structState);
}