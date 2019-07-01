using System;
using System.Collections.Generic;
using UnityEngine;


public class EventManager:Singleton<EventManager>
{
    private Dictionary<Enum, Action<EventBase>> HandlerList = new Dictionary<Enum, Action<EventBase>>();

    public void SendEvent(EventBase eventBase)
    {
        Action<EventBase> action;
        HandlerList[eventBase.eid].Invoke(eventBase);
    }
    /// <summary>
    /// 注册监听者
    /// </summary>
    /// <param name="e"></param>
    /// <param name="action"></param>
    public void Registration(Enum e, Action<EventBase> action)
    {
        if (!HandlerList.ContainsKey(e))
        {
            HandlerList.Add(e, action);
        }
        else
        {
            HandlerList[e] += action;
        }
    }
    /// <summary>
    /// 注册监听者(监听多个Enum)
    /// </summary>
    /// <param name="action"></param>
    /// <param name="enums"></param>
    public void Registration(Action<EventBase> action,params Enum[] enums)
    {
        
        for (int i = 0; i < enums.Length; i++)
        {
            Registration(enums[i], action);
        }
    }
    /// <summary>
    /// 注销监听者
    /// </summary>
    public void Cancellation(Enum e, Action<EventBase> action)
    {
        if (HandlerList.ContainsKey(e))
        {
            HandlerList[e] -= action;
        }
    }
    public void Cancellation(Action<EventBase> action,params Enum[] enums)
    {
        for (int i = 0; i < enums.Length; i++)
        {
            Cancellation(enums[i], action);
        }
    }
}

public interface IEventHandler
{
    void HandlerEvent(EventBase eventid);
}