using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class tttt : MonoBehaviour
{
    //123456
    private int[] bytes=new int[1024];
    private int[] tmp=new int[1012];
    // Start is called before the first frame update
    void Start()
    {
        DateTime startTime = DateTime.Now;    
        for (int i = 0; i < 10000000; i++)
        {

            Array.Copy(bytes, 12, tmp, 0, 1012);
        }
        print((DateTime.Now - startTime).ToString());
    }

    // Update is called once per frame
    void Update()
    {
        
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
}
