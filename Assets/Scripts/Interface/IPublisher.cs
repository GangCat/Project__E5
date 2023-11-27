using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ������ �������̽�
/// </summary>
public interface IPublisher
{
    /// <summary>
    /// ���Ŀ�� �ڽ��� ����ϴ� �Լ�
    /// </summary>
    void RegisterBroker();
    /// <summary>
    /// ���Ŀ���� �޽����� �����ϴ� �Լ�
    /// </summary>
    /// <param name="_message"></param>
    void PushMessageToBroker(EMessageType _message);
}
