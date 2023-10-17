using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICancleSubject
{
    /// <summary>
    /// �Ͻ����� ���� ������ ��� �޼ҵ�
    /// </summary>
    /// <param name="_observer"></param>
    void RegisterCancleObserver(ICancleObserver _observer);

    /// <summary>
    /// �Ͻ����� ���� ������ ���� �޼ҵ�
    /// </summary>
    /// <param name="_observer"></param>
    void RemoveCancleObserver(ICancleObserver _observer);

    /// <summary>
    /// �Ͻ����� ���� ���� �޼ҵ�
    /// </summary>
    void ToggleCancle();
}
