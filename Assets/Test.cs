using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Test : MonoBehaviour, IEventHandler
{
    public Texture2D texture;

    public void HandlerEvent(EventBase eventid)
    {
        switch ((MessageType)eventid.eid)
        {
            case MessageType.Test1:
                print("wo " + (eventid as EventData<Package>).args[0].GetString());
                break;
            case MessageType.Test2:
                print("wo " + (eventid as EventData<Package>).args[0].GetString());
                break;
            default:
                break;
        }

    }

    private void Start()
    {
        NetworkManager.Instance.Registration(HandlerEvent, MessageType.Test1, MessageType.Test2);
    }
    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Space))
        {
            Package package = new Package( 0, "test");
            EventData<Package>.CreateEvent(PackageDir.send, package).Send();
            Package package1 = new Package((int)MessageType.Test2, "哈哈哈啊哈哈哈哈哈哈啊哈哈哈哈哈哈啊哈哈哈哈哈");
            EventData<Package>.CreateEvent(PackageDir.send, package1).Send();
        }
    }
}
