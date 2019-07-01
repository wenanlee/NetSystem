using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventTest : MonoBehaviour, IEventHandler
{
    /// <summary>
    /// 事件处理器
    /// </summary>
    /// <param name="eventid"></param>
    public void HandlerEvent(EventBase eventid)
    {
        EventData<string> eventData = (EventData<string>)eventid;
        print("user:"+eventData.args[0] +"    password:"+ eventData.args[1]);
    }
    /// <summary>
    /// 订阅事件
    /// </summary>
    private void Start()
    {
        EventManager.Instance.Registration(HandlerEvent, MessageType.login);
        Send();
    }
    /// <summary>
    /// 发送事件
    /// </summary>
    private void Send()
    {
        EventData<string>.CreateEvent(MessageType.login, "122216637", "password").Send();
    }
}
