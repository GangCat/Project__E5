using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 구독자 인터페이스
/// </summary>
public interface ISubscriber
{
    /// <summary>
    /// 특정 발행자에 자신을 등록하는 함수
    /// </summary>
    void Subscribe();
    /// <summary>
    /// 구독한 발행자의 메시지를 받는 함수.
    /// </summary>
    /// <param name="_message"></param>
    void ReceiveMessage(EMessageType _message);
}
