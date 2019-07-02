using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class NetworkUtils
{
    public static IPAddress GetLocalIP()
    {
        IPAddress AddressIP=IPAddress.Any;
        foreach (IPAddress iPAddress in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
        {
            if (iPAddress.AddressFamily.ToString() == "InterNetwork")
            {
                return iPAddress;
            }
        }
        return AddressIP;
    }
    public static string GetLocalIPString()
    {
        return GetLocalIP().ToString();
    }
    public static byte[] Int2ByteArray(int value)
    {
        byte[] buffer = new byte[4];
        buffer[0] = (byte)((value >> 24));
        buffer[1] = (byte)((value >> 16));
        buffer[2] = (byte)((value >> 8));
        buffer[3] = (byte)(value);
        return buffer;
    }
    public static int ByteArray2Int(byte[] bytes,int startIndex)
    {
        return (bytes[startIndex] << 24) + (bytes[startIndex + 1] << 16) + (bytes[startIndex + 2] << 8) + bytes[startIndex + 3];
    }
}
