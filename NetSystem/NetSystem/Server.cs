using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NetSystem
{
    public static class FileServer
    {
        private static Socket serverSocket;
        public static void Init()
        {
            //服务器IP地址
            IPAddress ip = IPAddress.Parse("127.0.0.1");
            int myProt = 10086;
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            serverSocket.Bind(new IPEndPoint(ip, myProt));  //绑定IP地址：端口
            serverSocket.Listen(10);    //设定最多10个排队连接请求
            Console.WriteLine("启动监听{0}成功", serverSocket.LocalEndPoint.ToString());
            //通过Clientsoket发送数据
            Thread myThread = new Thread(ListenClientConnect);
            myThread.Start();
        }
        public static void Exit()
        {
            serverSocket.Close();
            serverSocket = null;
        }
        private static void ListenClientConnect()
        {
            while (true)
            {
                if (serverSocket != null)
                {
                    try
                    {
                        Socket clientSocket = serverSocket.Accept();
                        Thread receiveThread = new Thread(Create);
                        receiveThread.Start(clientSocket);
                    }
                    catch
                    {
                        break;
                    }
                }
            }
        }
        public static void Create(object clientSocket)
        {
            Socket client = clientSocket as Socket;
            //获得客户端节点对象
            IPEndPoint clientep = (IPEndPoint)client.RemoteEndPoint;

            //获得[文件名]   
            string SendFileName = System.Text.Encoding.Unicode.GetString(TransferFiles.ReceiveVarData(client));


            //获得[包的大小]   
            string bagSize = System.Text.Encoding.Unicode.GetString(TransferFiles.ReceiveVarData(client));

            //获得[包的总数量]   
            int bagCount = int.Parse(System.Text.Encoding.Unicode.GetString(TransferFiles.ReceiveVarData(client)));

            //获得[最后一个包的大小]   
            string bagLast = System.Text.Encoding.Unicode.GetString(TransferFiles.ReceiveVarData(client));


            string fullPath = Path.Combine(Environment.CurrentDirectory, SendFileName);
            //创建一个新文件   
            FileStream MyFileStream = new FileStream(fullPath, FileMode.Create, FileAccess.Write);

            //已发送包的个数   
            int SendedCount = 0;
            while (true)
            {

                byte[] data = TransferFiles.ReceiveVarData(client);
                if (data.Length == 0)
                {
                    break;
                }
                else
                {
                    SendedCount++;
                    //将接收到的数据包写入到文件流对象   
                    MyFileStream.Write(data, 0, data.Length);
                    //显示已发送包的个数     

                }
            }
            //关闭文件流   
            MyFileStream.Close();
            //关闭套接字   
            client.Close();
            Console.WriteLine(SendFileName + "接收完毕！");
        }
    }
}
