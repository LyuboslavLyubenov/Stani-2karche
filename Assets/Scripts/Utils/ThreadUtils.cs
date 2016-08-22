using System;
using System.Collections.Generic;
using UnityEngine;

public class ThreadUtils : MonoBehaviour
{
    readonly Queue<Action> MethodsQueue = new Queue<Action>();
    readonly object MyLock = new object();

    static ThreadUtils instance;

    public static ThreadUtils Instance
    {
        get
        {
            if (instance == null)
            {
                var gameObject = new GameObject();
                var threadUtilsComponent = gameObject.AddComponent<ThreadUtils>();

                instance = threadUtilsComponent;
                gameObject.name = "ThreadUtils";
            }

            return instance;
        }
    }

    public void RunOnMainThread(Action method)
    {
        lock (MyLock)
        {
            MethodsQueue.Enqueue(method);        
        }
    }

    void Update()
    {
        lock (MyLock)
        {
            if (MethodsQueue.Count < 1)
            {
                return;
            }

            var methodToRun = MethodsQueue.Dequeue();
            methodToRun();    
        }
    }
}
