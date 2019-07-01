# NetworkSystem

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