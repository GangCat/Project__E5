using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���¸ӽ��� �������̽�
/// </summary>
public interface IState
{
    /// <summary>
    /// ���׽�Ʈ ���۽� 1�� ȣ��
    /// </summary>
    /// <param name="_structState"></param>
    public void Start(ref SUnitState _structState);
    /// <summary>
    /// ������Ʈ ������Ʈ�� �� ������ ȣ��
    /// </summary>
    /// <param name="_structState"></param>
    public void Update(ref SUnitState _structState);
    /// <summary>
    /// ������Ʈ ����� 1�� ȣ��
    /// </summary>
    /// <param name="_structState"></param>
    public void End(ref SUnitState _structState);
}