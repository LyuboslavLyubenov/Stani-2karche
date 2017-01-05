namespace Assets.Scripts.Utils
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    using UnityEngine;
    using CielaSpike.Thread_Ninja;

    public class ThreadUtils : MonoBehaviour
    {
        private readonly Queue<Action> methodsQueue = new Queue<Action>();

        private readonly object myLock = new object();

        private static readonly object instanceLock = new object();
        private static ThreadUtils instance;

        public static ThreadUtils Instance
        {
            get
            {
                lock (instanceLock)
                {
                    if (instance == null)
                    {
                        var gameObject = new GameObject("ThreadUtils");
                        var threadUtilsComponent = gameObject.AddComponent<ThreadUtils>();

                        instance = threadUtilsComponent;
                    }
                }

                return instance;
            }
        }
        
        // ReSharper disable once ArrangeTypeMemberModifiers
        void Update()
        {
            lock (this.myLock)
            {
                if (this.methodsQueue.Count < 1)
                {
                    return;
                }

                var methodToRun = this.methodsQueue.Dequeue();
                methodToRun();    
            }
        }

        public void CancelThread(IEnumerator coroutine)
        {
            this.StopCoroutine(coroutine);
        }
        
        public void RunOnMainThread(IEnumerator coroutine)
        {
            this.StartCoroutine(coroutine);
        }

        public void RunOnMainThread(Action method)
        {
            lock (this.myLock)
            {
                this.methodsQueue.Enqueue(method);        
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