namespace Assets.Scripts.Utils
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    using UnityEngine;
    using CielaSpike.Thread_Ninja;

    using UnityEngine.SceneManagement;

    public class ThreadUtils : MonoBehaviour
    {
        private readonly Queue<Action> methodsQueue = new Queue<Action>();
        private readonly Queue<CoroutineTask> coroutinesQueue = new Queue<CoroutineTask>();

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

        void Start()
        {
            SceneManager.activeSceneChanged += OnActiveSceneChanged;
        }

        void OnDisable()
        {
            this.StopAllThreads();
        }

        void OnApplicationQuit()
        {
            this.StopAllThreads();
        }

        private void StopAllThreads()
        {
            lock (this.myLock)
            {
                this.methodsQueue.Clear();
                this.coroutinesQueue.Clear();
                this.StopAllCoroutines();
            }
        }

        private void OnActiveSceneChanged(Scene arg0, Scene scene)
        {
            this.StopAllThreads();
            SceneManager.activeSceneChanged -= OnActiveSceneChanged;
        }

        // ReSharper disable once ArrangeTypeMemberModifiers
        void Update()
        {
            lock (this.myLock)
            {
                if (this.methodsQueue.Count >= 1)
                {
                    this.ExecuteNextMethod();
                }

                if (this.coroutinesQueue.Count >= 1)
                {
                    this.ExecuteNextCoroutine();
                }
            }
        }

        private void ExecuteNextMethod()
        {
            var methodToRun = this.methodsQueue.Dequeue();
            methodToRun();
        }

        private void ExecuteNextCoroutine()
        {
            var coroutineToRun = this.coroutinesQueue.Dequeue();

            if (coroutineToRun.Coroutine == null)
            {
                return;
            }

            if (coroutineToRun.ExecuteOnUnityThread)
            {
                this.StartCoroutine(coroutineToRun.Coroutine);
            }
            else
            {
                this.StartCoroutineAsync(coroutineToRun.Coroutine);
            }
        }

        private void _CancelCoroutine(IEnumerator coroutine)
        {
            lock (this.myLock)
            {
                var coroutineInQueue = this.coroutinesQueue.FirstOrDefault(c => c.Coroutine == coroutine);

                if (coroutineInQueue != null)
                {
                    coroutineInQueue.Cancel();
                }
            }

            this.StopCoroutine(coroutine);
        }

        public void CancelCoroutine(IEnumerator coroutine)
        {
            RunOnMainThread(() => _CancelCoroutine(coroutine));
        }

        public void RunOnMainThread(IEnumerator coroutine)
        {
            lock (this.myLock)
            {
                var task = new CoroutineTask(coroutine, true);
                this.coroutinesQueue.Enqueue(task);
            }
        }

        public void RunOnMainThread(Action method)
        {
            lock (this.myLock)
            {
                this.methodsQueue.Enqueue(method);
            }
        }

        public void RunOnBackgroundThread(IEnumerator coroutine)
        {
            lock (this.myLock)
            {
                var task = new CoroutineTask(coroutine, false);
                this.coroutinesQueue.Enqueue(task);
            }
        }
    }

    public class CoroutineTask
    {
        public IEnumerator Coroutine
        {
            get;
            private set;
        }

        public bool ExecuteOnUnityThread
        {
            get;
            private set;
        }

        public CoroutineTask(IEnumerator coroutine, bool executeOnUnityThread)
        {
            if (coroutine == null)
            {
                throw new ArgumentNullException("coroutine");
            }

            this.Coroutine = coroutine;
            this.ExecuteOnUnityThread = executeOnUnityThread;
        }

        public void Cancel()
        {
            this.Coroutine = null;
        }
    }
}