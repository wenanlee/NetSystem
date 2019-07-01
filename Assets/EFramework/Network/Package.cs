using System;
using System.Linq;
using System.Net;
using System.Text;

public class Package
{
    public static int messageIndex=0;
    public IPEndPoint address;
    public int type;
    public int index;
    public byte[] msg;
    public Package()
    {
        address = new IPEndPoint(IPAddress.Any, 0);
        type = 0;
        index = 0;
        msg = null;
    }
    public Package(string ip, int port, int type, byte[] msg)
    {
        Format(ip, port, type, msg);
    }
    public Package(string ip, int port, int type, string msg)
    {
        Format(ip, port, type, Encoding.UTF8.GetBytes(msg));
    }
    public Package(string ip, int port, int type, int msg)
    {
        Format(ip, port, type, BitConverter.GetBytes(msg));
    }
    private void Format(string ip, int port, int type, byte[] msg)
    {
        address = new IPEndPoint(IPAddress.Parse(ip), port);
        this.type = type;
        this.index = messageIndex;
        this.msg = msg;
    }
    public string GetString()
    {
        if (msg == null)
            return "";
        return Encoding.UTF8.GetString(msg);
    }
    public int GetInt()
    {
        if (msg == null)
            return 0;
        return BitConverter.ToInt32(msg, 0);
    }
    public byte[] Pack()
    {
        byte[] msg = BitConverter.GetBytes(type);
        msg = msg.Concat(BitConverter.GetBytes(index)).Concat(this.msg).ToArray();
        return msg;
    }

    public Package UnPack(byte[] bytes)
    {
        type = BitConverter.ToInt32(bytes.Skip(0).Take(4).ToArray(), 0);
        index = BitConverter.ToInt32(bytes.Skip(4).Take(4).ToArray(), 0);
        msg = bytes.Skip(8).ToArray();
        return this;
    }
}