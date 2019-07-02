using System;
using System.Linq;
using System.Net;
using System.Text;
using UnityEngine;

public class Package
{
    public static int messageIndex = 0;                 //总的消息编号
    //------------------------------包内容-------------------------------------------------
    public int size;                                    //包大小
    public int type;                                    //消息类型
    public int index;                                   //当前消息编号
    public byte[] msg;                                  //真正的消息
    private byte[] fullData;                            //编码过后的信息
    //小松只有9分了

    /// <summary>
    /// 初始化包
    /// </summary>
    public Package()
    {
        type = 0;
        index = 0;
        msg = null;
    }
    /// <summary>
    /// 初始化包
    /// </summary>
    /// <param name="ip">IP地址</param>
    /// <param name="port">端口号</param>
    /// <param name="type">消息类型</param>
    /// <param name="msg">消息体</param>
    public Package(int type, byte[] msg)
    {
        Format(type, msg);
    }
    /// <summary>
    /// 初始化包
    /// </summary>
    /// <param name="ip">IP地址</param>
    /// <param name="port">端口号</param>
    /// <param name="type">消息类型</param>
    /// <param name="msg">消息体</param>
    public Package(int type, string msg)
    {
        Format(type, Encoding.UTF8.GetBytes(msg));
    }
    /// <summary>
    /// 初始化包
    /// </summary>
    /// <param name="ip">IP地址</param>
    /// <param name="port">端口号</param>
    /// <param name="type">消息类型</param>
    /// <param name="msg">消息体</param>
    public Package(int type, int msg)
    {
        Format(type, BitConverter.GetBytes(msg));
    }
    /// <summary>
    /// 格式化消息
    /// </summary>
    /// <param name="ip">IP地址</param>
    /// <param name="port">端口号</param>
    /// <param name="type">消息类型</param>
    /// <param name="msg">消息体</param>
    private void Format(int type, byte[] msg)
    {
        this.type = type;
        this.index = messageIndex;
        this.msg = msg;
        Pack();
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
    /// <summary>
    /// 编码
    /// </summary>
    /// <returns></returns>
    public byte[] Pack()
    {
        byte[] packHead = new byte[4 * 3];
        size = msg.Length + packHead.Length;      //3个int（4）型数据 size type index 
        //包大小
        packHead[0] = (byte)((size >> 24));
        packHead[1] = (byte)((size >> 16));
        packHead[2] = (byte)((size >> 8));
        packHead[3] = (byte)(size);
        //包类型
        packHead[4] = (byte)((type >> 24));
        packHead[5] = (byte)((type >> 16));
        packHead[6] = (byte)((type >> 8));
        packHead[7] = (byte)(type);
        //包编号
        packHead[8] = (byte)((index >> 24));
        packHead[9] = (byte)((index >> 16));
        packHead[10] = (byte)((index >> 8));
        packHead[11] = (byte)(index);

        fullData = packHead.Concat(msg).ToArray();
        return fullData;
    }
    public int GetLength()
    {
        if (size == 0)
        {
            Debug.Log("消息没有打包 Pack()");
            return 0;
        }
        else
        {
            return size;
        }
    }
    public Package UnPack()
    {
        UnPack(fullData);
        return this;
    }
    public Package UnPack(byte[] bytes)
    {
        size = (bytes[0] << 24) + (bytes[1] << 16) + (bytes[2] << 8) + bytes[3];
        type = (bytes[4] << 24) + (bytes[5] << 16) + (bytes[6] << 8) + bytes[7];
        index = (bytes[8] << 24) + (bytes[9] << 16) + (bytes[10] << 8) + bytes[11];
        msg = new byte[bytes.Length - 12];
        Array.Copy(bytes, 12, msg, 0, msg.Length);
        return this;
    }
}