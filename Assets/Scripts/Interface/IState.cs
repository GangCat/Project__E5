using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 상태머신의 인터페이스
/// </summary>
public interface IState
{
    public void Start(ref SUnitState _structState);
    public void Update(ref SUnitState _structState);
    public void End(ref SUnitState _structState);
}