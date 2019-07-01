# EventSystem
事件系统

接收发送事件
```c#
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
```
发送接收网络信息

````c#
public class NetworkTest : MonoBehaviour, IEventHandler
{
    public void HandlerEvent(EventBase eventid)
    {
        EventData<Package> msg = (EventData<Package>)eventid;
        Debug.Log(BitConverter.ToInt32(msg.args[0].msg, 0));
    }

    void Start()
    {
        print(NetworkManager.Instance.ToString());
        EventManager.Instance.Registration(HandlerEvent, MessageType.login);
        Send();
    }
    void Send()
    {
        Package package = new Package("127.0.0.1", 60000, (int)MessageType.login, BitConverter.GetBytes(12));
        EventData<Package>.CreateEvent(PackageDir.send, package).Send();
    }
}
````