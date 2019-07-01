using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkManager : Singleton<NetworkManager>, IEventHandler
{
    //private Queue<Package> packages = new Queue<Package>();
    private Network.Client client;
    private Network.Server server;
    protected override void InitSingleton()
    {
        EventManager.Instance.Registration(HandlerEvent, PackageDir.send, PackageDir.receive);
        client = new Network.Client();
        server = new Network.Server();
        client.Init();
        server.Init();
    }

    /// <summary>
    /// 注册监听者
    /// </summary>
    /// <param name="e"></param>
    /// <param name="action"></param>
    public void Registration(Enum e, Action<EventBase> action)
    {
        EventManager.Instance.Registration(e, action);
    }
    /// <summary>
    /// 注册监听者(监听多个Enum)
    /// </summary>
    /// <param name="action"></param>
    /// <param name="enums"></param>
    public void Registration(Action<EventBase> action, params Enum[] enums)
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
        EventManager.Instance.Registration(e, action);
    }
    /// <summary>
    /// 注销多个监听者
    /// </summary>
    /// <param name="action"></param>
    /// <param name="enums"></param>
    public void Cancellation(Action<EventBase> action, params Enum[] enums)
    {
        for (int i = 0; i < enums.Length; i++)
        {
            Cancellation(enums[i], action);
        }
    }

    public void HandlerEvent(EventBase eventid)
    {
        switch ((PackageDir)eventid.eid)
        {
            case PackageDir.send:
                Send((EventData<Package>)eventid);
                break;
            case PackageDir.receive:
                Receive((EventData<Package>)eventid);
                break;
            default:
                break;
        }
    }

    private void Receive(EventData<Package> msg)
    {
        //Debug.Log(msg.args[0].type);
        foreach (var item in msg.args)
        {
            Debug.Log((MessageType)item.type);
            EventData<Package>.CreateEvent((MessageType)item.type, item).Send();
        }
    }

    private void Send(EventData<Package> msg)
    {
        foreach (Package item in msg.args)
        {
            client.Send(item);
        }
    }
}
