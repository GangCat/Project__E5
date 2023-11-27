using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ������ �������̽�
/// </summary>
public interface ISubscriber
{
    /// <summary>
    /// Ư�� �����ڿ� �ڽ��� ����ϴ� �Լ�
    /// </summary>
    void Subscribe();
    /// <summary>
    /// ������ �������� �޽����� �޴� �Լ�.
    /// </summary>
    /// <param name="_message"></param>
    void ReceiveMessage(EMessageType _message);
}
