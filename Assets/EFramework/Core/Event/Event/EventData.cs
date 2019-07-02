using UnityEngine;
using System.Collections;
using System;

public class EventData<T> : EventBase
{
    public T[] args = null;
    public EventData(Enum eid, params T[] args)
    {
        this.eid = eid;
        this.args = args;
    }
    public static EventData<T> CreateEvent(Enum e, params T[] args)
    {
        EventData<T> eventBase = new EventData<T>(e, args);
        return eventBase;
    }
}
public class EventData:EventBase
{
    public object args = null;
    public EventData(Enum eid, params object[] args)
    {
        this.eid = eid;
        this.args = args;
    }
    public static EventData CreateEvent(Enum e, params object[] args)
    {
        EventData eventBase = new EventData(e, args);
        return eventBase;
    }
}
public class EventBase
{
    public Enum eid;
    public void Send()
    {
        if (EventManager.Instance != null) EventManager.Instance.SendEvent(this);
    }
}