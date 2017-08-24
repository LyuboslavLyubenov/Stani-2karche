namespace Utils
{
    using System;
    using System.Collections.Generic;
    using System.Timers;

    using UnityEngine.SceneManagement;

    public class TimerUtils
    {
        private static readonly List<Timer> allTimers = new List<Timer>();

        private TimerUtils()
        {
            SceneManager.activeSceneChanged += OnActiveSceneChanged;
        }

        private void OnActiveSceneChanged(Scene oldScene, Scene newScene)
        {
            for (int i = 0; i < allTimers.Count; i++)
            {
                var timer = allTimers[i];
                timer.Stop();
                timer.Dispose();
            }

            allTimers.Clear();
        }

        /// <summary>
        /// Creates timer executing {method} after {seconds} seconds
        /// </summary>
        /// <returns>Created timer</returns>
        public static Timer_ExecuteMethodAfterTime ExecuteAfter(float seconds, Action method)
        {
            var timer = new Timer_ExecuteMethodAfterTime(seconds * 1000)
            {
                Method = method,
                AutoDispose = true
            };

            allTimers.Add(timer);

            return timer;
        }
        
        /// <summary>
        /// Creates timer executing {method} every {seconds} seconds
        /// </summary>
        /// <returns>Created timer</returns>
        public static Timer_ExecuteMethodEverySeconds ExecuteEvery(float seconds, Action method)
        {
            var timer = new Timer_ExecuteMethodEverySeconds(seconds * 1000)
            {
                Method = method
            };

            allTimers.Add(timer);

            return timer;
        }

        /// <summary>
        /// Creates timer executing {method} when {condition} is met
        /// </summary>
        public static Timer_ExecuteMethodWhen ExecuteWhen(Func<bool> condition, Action method)
        {
            return new Timer_ExecuteMethodWhen(condition, method);
        }
    }
}