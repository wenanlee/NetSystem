using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace NetSystem
{
    public static class FileClient
    {
        public static bool SendFile(string IP, int Port, string fullPath)
        {
            //创建一个文件对象
            FileInfo EzoneFile = new FileInfo(fullPath);
            //打开文件流
            FileStream EzoneStream = EzoneFile.OpenRead();

            //包的大小
            int PacketSize = 10000;

            //包的数量
            int PacketCount = (int)(EzoneStream.Length / ((long)PacketSize));

            //最后一个包的大小
            int LastDataPacket = (int)(EzoneStream.Length - ((long)(PacketSize * PacketCount)));

            //指向远程服务端节点
            IPEndPoint ipep = new IPEndPoint(IPAddress.Parse(IP), Port);

            //创建套接字
            Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            //连接到发送端
            try
            {
                client.Connect(ipep);
            }
            catch
            {
                Console.WriteLine("连接服务器失败！");
                return false;
            }
            //获得客户端节点对象
            IPEndPoint clientep = (IPEndPoint)client.RemoteEndPoint;


            //发送[文件名]到客户端
            TransferFiles.SendVarData(client, Encoding.Unicode.GetBytes(EzoneFile.Name));

            //发送[包的大小]到客户端
            TransferFiles.SendVarData(client, Encoding.Unicode.GetBytes(PacketSize.ToString()));

            //发送[包的总数量]到客户端
            TransferFiles.SendVarData(client, Encoding.Unicode.GetBytes(PacketCount.ToString()));

            //发送[最后一个包的大小]到客户端
            TransferFiles.SendVarData(client, Encoding.Unicode.GetBytes(LastDataPacket.ToString()));

            bool isCut = false;
            //数据包
            byte[] data = new byte[PacketSize];
            //开始循环发送数据包
            for (int i = 0; i < PacketCount; i++)
            {
                //从文件流读取数据并填充数据包
                EzoneStream.Read(data, 0, data.Length);
                //发送数据包
                if (TransferFiles.SendVarData(client, data) == 3)
                {
                    isCut = true;
                    return false;
                    break;
                }

            }

            //如果还有多余的数据包，则应该发送完毕！
            if (LastDataPacket != 0)
            {
                data = new byte[LastDataPacket];
                EzoneStream.Read(data, 0, data.Length);
                TransferFiles.SendVarData(client, data);
            }

            //关闭套接字
            client.Close();
            //关闭文件流
            EzoneStream.Close();
            if (!isCut)
            {
                return true;
            }
            return false;
        }
    }
}
