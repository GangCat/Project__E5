using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 발행자 인터페이스
/// </summary>
public interface IPublisher
{
    /// <summary>
    /// 브로커에 자신을 등록하는 함수
    /// </summary>
    void RegisterBroker();
    /// <summary>
    /// 브로커에게 메시지를 전달하는 함수
    /// </summary>
    /// <param name="_message"></param>
    void PushMessageToBroker(EMessageType _message);
}
