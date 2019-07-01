
using UnityEngine;

public abstract class Singleton<T> where T : Singleton<T>, new()
{
    private static T ms_instance = default(T);

    public static T Instance
    {
        get
        {
            if (ms_instance == null)
            {
                ms_instance = new T();
                ms_instance.InitSingleton();
            }
            return ms_instance;
        }
    }

    protected virtual void InitSingleton()
    {

    }
}

//**********************************************************************


