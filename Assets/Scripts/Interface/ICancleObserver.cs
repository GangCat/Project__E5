using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICancleObserver
{
    /// <summary>
    /// ��ü�κ��� ������Ʈ�� �޴� �޼ҵ�
    /// </summary>
    /// <param name="_isPause"></param>
    void CheckCancle(bool _isPause);
}