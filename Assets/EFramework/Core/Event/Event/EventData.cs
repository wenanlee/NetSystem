using UnityEngine;
using System.Collections;
using System;

public class EventData<T> : EventBase
{
    public T[] args = null;
    public EventData(Enum eid)
    {
        this.eid = eid;
    }
    public EventData(Enum eid, params T[] args)
    {
        this.eid = eid;
        this.args = args;
    }
    public static EventData<T> CreateEvent(Enum e)
    {
        EventData<T> eventBase = new EventData<T>(e);
        return eventBase;
    }
    public static EventData<T> CreateEvent(Enum e, params T[] args)
    {
        EventData<T> eventBase = new EventData<T>(e, args);
        return eventBase;
    }
}
//public class EventString : EventData<string>
//{
//    public EventString(Enum eid) : base(sg)
//    {
//        this.eid = eid;
//    }
//    public EventData(Enum eid, params T[] args)
//    {
//        this.eid = eid;
//        this.args = args;
//    }
//    public static EventData<T> CreateEvent(Enum e)
//    {
//        EventData<T> eventBase = new EventData<T>(e);
//        return eventBase;
//    }
//    public static EventData<T> CreateEvent(Enum e, params T[] args)
//    {
//        EventData<T> eventBase = new EventData<T>(e, args);
//        return eventBase;
//    }
//}
public class EventBase
{
    public Enum eid;
    public void Send()
    {
        if (EventManager.Instance != null) EventManager.Instance.SendEvent(this);
    }
}