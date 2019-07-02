using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class Network
{
    private const int BUFF_SIZE = 128;
    public class UDPClient
    {
        UdpClient client;

        public void Init(string ip="127.0.0.1",int port=60000)
        {
            client = new UdpClient();
            client.Connect(ip, port);
            client.BeginReceive(Receive, client);
        }
        public void Send(Package package)
        {
            byte[] msgBytes = package.Pack();
            client.Send(msgBytes, msgBytes.Length);
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

    public class UDPServer
    {
        UdpClient server;

        public void Init(int port = 60000)
        {
            server = new UdpClient(port);
            //ThreadPool.QueueUserWorkItem(Receive);
            server.BeginReceive(Receive, server);
        }

        private void Receive(IAsyncResult ar)
        {
            UdpClient u = (UdpClient)ar.AsyncState;
            IPEndPoint e = new IPEndPoint(IPAddress.Any, 0);
            byte[] receiveBytes = u.EndReceive(ar, ref e);
            new EventData<Package>(PackageDir.receive, new Package().UnPack(receiveBytes)).Send();
            server.BeginReceive(Receive, server);
        }
    }
    public class TCPClient
    {
        private TcpClient tcpclient = null;
        byte[] result;
        public void Init(string ip,int port=60001)
        {
            tcpclient = new TcpClient();
            //连接服务器
            tcpclient.BeginConnect(IPAddress.Parse(ip), port, ConnectCallback, tcpclient);
        }
        /// <summary>
        /// 连接回调
        /// </summary>
        /// <param name="ar"></param>
        private void ConnectCallback(IAsyncResult ar)
        {
            TcpClient client = (TcpClient)ar.AsyncState;
            try
            {
                if (client.Connected)
                {
                    //连接成功
                    client.EndConnect(ar);
                    //开始接受数据
                    ReceiveData(client);

                }
            }
            catch (Exception e)
            {
                Debug.Log("ConnectCallback：" + e.Message);
            }
        }
        /// <summary>
        /// 接收数据
        /// </summary>
        /// <param name="client"></param>
        private void ReceiveData(TcpClient client)
        {
            NetworkStream stream = client.GetStream();

            result = new byte[client.Available];

            try
            {
                stream.BeginRead(result, 0, result.Length, ReadCallBack, stream);
            }
            catch (Exception e)
            {
                Debug.Log("ReceiveData:" + e.Message);
            }
        }
        /// <summary>
        /// 接受消息回调
        /// </summary>
        /// <param name="ar"></param>
        private void ReadCallBack(IAsyncResult ar)
        {
            NetworkStream stream;
            try
            {
                stream = (NetworkStream)ar.AsyncState;
                stream.EndRead(ar);
                byte[] sizeData = new byte[4];
                stream.Read(sizeData, 0, sizeData.Length);

                int size = BitConverter.ToInt32(sizeData,0);
                byte[] data = new byte[size];
                stream.Read(data, 0, data.Length);
                EventData<Package>.CreateEvent(PackageDir.receive,new Package().UnPack(data)).Send();
            }
            catch (IOException e)
            {
                Debug.Log("远程服务器关闭"+e.Message);
            }

        }
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="data"></param>
        public void Send(Package package)
        {
            if (tcpclient.Connected)
            {
                NetworkStream stream = tcpclient.GetStream();
                byte[] data = package.Pack();
                stream.BeginWrite(data, 0, data.Length, SendCallback, stream);
            }
        }
        /// <summary>
        /// 发送消息回调
        /// </summary>
        /// <param name="ar"></param>
        private void SendCallback(IAsyncResult ar)
        {
            NetworkStream stream = (NetworkStream)ar.AsyncState;
            stream.EndWrite(ar);
        }
    }
    public class TCPServer
    {
        private TcpListener server;
        byte[] buffer = new byte[1024];
        /// <summary>
        /// 接受消息
        /// </summary>
        List<byte> receiveDataList = new List<byte>();
        public void Init(int port=60001)
        {
            server = new TcpListener(new IPEndPoint(IPAddress.Any, port));
            //开启侦听，最多只能连接20个客户端数目
            server.Start(20);
            server.BeginAcceptSocket(ClientConnect, server);
        }
        /// <summary>
        /// 客户端连接
        /// </summary>
        /// <param name="ar"></param>
        private void ClientConnect(IAsyncResult ar)
        {
            try
            {
                TcpListener listener = (TcpListener)ar.AsyncState;
                Socket client = listener.EndAcceptSocket(ar);
                Debug.Log(client.RemoteEndPoint.ToString() + "连接成功");
                //侦听结束后开始接收数据
                ReceiveData(client);
                //重新侦听是否有新的客户端连接
                //如果这里不写  那么只能侦听一次
                server.BeginAcceptSocket(ClientConnect, server);
            }
            catch (Exception e)
            {

                Debug.Log("ClientConnect" + e.Message);
            }
        }
        /// <summary>
        /// 接受数据的回调
        /// </summary>
        /// <param name="client"></param>
        private void ReceiveData(Socket client)
        {
            SocketError socketError;
            client.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, out socketError, ReceiveCallback, client);
        }
        /// <summary>
        /// 一次接收的数据的长度
        /// </summary>
        int receLen = 0;
        private void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                SocketError socketError;
                Socket client = (Socket)ar.AsyncState;
                receLen = client.EndReceive(ar, out socketError);
                if (receLen > 0)
                {
                    lock (receiveDataList)
                    {
                        //处理接收到的数据

                        OnReceiveData(client);
                    }

                }
                else//当receLen<=0的时候表示客户端断开和服务器的连接
                {
                    Debug.Log(client.LocalEndPoint.ToString() + "关闭了");
                }
                //重新接受数据
                ReceiveData(client);
            }
            catch (Exception e)
            {
                Debug.Log("ReceiveCallback:" + e.Message + "   " + e.StackTrace);
            }
        }
        /// <summary>
        /// 接受数据
        /// </summary>
        /// <param name="socket"></param>
        private void OnReceiveData(Socket socket)
        {

            byte[] tempArr = new byte[receLen];
            Array.Copy(buffer, tempArr, receLen);
            receiveDataList.AddRange(tempArr);
            if (receiveDataList.Count >= 4)
            {
                byte[] dataArray = receiveDataList.GetRange(0, 4).ToArray();
                //int len = BitConverter.ToInt32(dataArray, 0);
                int len = NetworkUtils.ByteArray2Int(dataArray, 0);
                //Debug.Log("receiveDataList: " + receiveDataList.Count+"  == ?"+len);
                if (receiveDataList.Count >= len)
                {
                    Package package = new Package();
                    package.UnPack(receiveDataList.ToArray());

                    EventData<Package>.CreateEvent(PackageDir.receive, package).Send();
                }
            }
        }
        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="data"></param>
        public void Send(Socket socket,Package package)
        {
            SocketError socketError;
            byte[] data = package.Pack();
            socket.BeginSend(data, 0, data.Length, SocketFlags.None, out socketError, SendCallBack, socket);//异步发送数据
        }
        /// <summary>
        /// 发送数据的回调
        /// </summary>
        /// <param name="ar"></param>
        private void SendCallBack(IAsyncResult ar)
        {
            SocketError socketError;
            Socket client = (Socket)ar.AsyncState;
            client.EndSend(ar, out socketError);
        }
    }
}
public enum PackageDir
{
    send=100,receive
}