namespace Assets.Scripts.Utils
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    using UnityEngine;
    using CielaSpike.Thread_Ninja;

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

        void Update()
        {
            lock (this.MyLock)
            {
                if (this.MethodsQueue.Count < 1)
                {
                    return;
                }

                var methodToRun = this.MethodsQueue.Dequeue();
                methodToRun();    
            }
        }
        
        public void RunOnMainThread(Action method)
        {
            lock (this.MyLock)
            {
                this.MethodsQueue.Enqueue(method);        
            }
        }

        public void RunOnBackgroundThread(IEnumerator coroutine, out Task task)
        {
            this.StartCoroutineAsync(coroutine, out task);    
        }

        public void RunOnBackgroundThread(IEnumerator coroutine)
        {
            this.StartCoroutineAsync(coroutine);    
        }
    }
}
