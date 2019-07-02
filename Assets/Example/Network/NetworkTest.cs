using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class NetworkTest : MonoBehaviour, IEventHandler
{
    public void HandlerEvent(EventBase eventid)
    {
        EventData<Package> msg = (EventData<Package>)eventid;

        switch ((MessageType)msg.eid)
        {
            case MessageType.Test1:
                Debug.Log("Int： "+msg.args[0].GetInt());
                break;
            case MessageType.Test2:
                Debug.Log("Bytes： " + msg.args[0].msg.Length);
                break;
            case MessageType.Test3:
                Debug.Log("String： " + msg.args[0].GetString());
                break;
            default:
                break;
        }
    }

    void Start()
    {
        print(NetworkManager.Instance.ToString());
        EventManager.Instance.Registration(HandlerEvent, MessageType.Test1,MessageType.Test2,MessageType.Test3);
        Send();
    }
    void Send()
    {
        //int
        Package intPack = new Package((int)MessageType.Test1, 12);
        EventData<Package>.CreateEvent(PackageDir.send, intPack).Send();
        //小字节数组
        Package smallByteArrayPack = new Package((int)MessageType.Test2, new byte[10]);
        EventData<Package>.CreateEvent(PackageDir.send, smallByteArrayPack).Send();
        //大字节数组
        Package bigByteArrsyPack = new Package((int)MessageType.Test2, new byte[1024000]);
        EventData<Package>.CreateEvent(PackageDir.send, bigByteArrsyPack).Send();
        //字符串
        Package stringPack = new Package((int)MessageType.Test3, "test");
        EventData<Package>.CreateEvent(PackageDir.send, stringPack).Send();
    }
}
