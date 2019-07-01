using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class Network
{
    public class Client
    {
        UdpClient client;

        public void Init()
        {
            client = new UdpClient();
            client.BeginReceive(Receive, client);
        }
        public void Send(Package package)
        {
            byte[] msgBytes = package.Pack();
            client.Send(msgBytes, msgBytes.Length, package.address);
        }
        private void Receive(IAsyncResult ar)
        {
            UdpClient u = (UdpClient)ar.AsyncState;
            IPEndPoint e = new IPEndPoint(IPAddress.Any, 0);
            byte[] receiveBytes = u.EndReceive(ar, ref e);
            new EventData<Package>(PackageDir.receive, new Package().UnPack(receiveBytes)).Send();
            client.BeginReceive(Receive, client);
        }
        public void Close()
        {
            client.Close();
        }
    }
    public class Server
    {
        UdpClient server;

        public void Init(int port = 60000)
        {
            server = new UdpClient(port);
            //ThreadPool.QueueUserWorkItem(Receive);
            server.BeginReceive(Receive, server);
        }
        bool IsRun = true;
        IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);

        private void Receive(IAsyncResult ar)
        {
            UdpClient u = (UdpClient)ar.AsyncState;
            IPEndPoint e = new IPEndPoint(IPAddress.Any, 0);
            byte[] receiveBytes = u.EndReceive(ar, ref e);
            new EventData<Package>(PackageDir.receive, new Package().UnPack(receiveBytes)).Send();
            server.BeginReceive(Receive, server);
        }
    }
}
public enum PackageDir
{
    send=100,receive
}