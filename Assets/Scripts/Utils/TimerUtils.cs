namespace Assets.Scripts.Utils
{
    using System;
    using System.Timers;

    public class TimerUtils
    {
        private TimerUtils()
        {
        }

        /// <summary>
        /// Creates timer executing {method} after {seconds} seconds
        /// </summary>
        /// <returns>Created timer</returns>
        public static Timer ExecuteAfter(float seconds, Action method)
        {
            var timer = new Timer_ExecuteMethodAfterTime(seconds * 1000)
            {
                Method = method,
                AutoDispose = true
            };

            return timer;
        }
        
        /// <summary>
        /// Creates timer executing {method} every {seconds} seconds
        /// </summary>
        /// <returns>Created timer</returns>
        public static Timer ExecuteEvery(float seconds, Action method)
        {
            var timer = new Timer_ExecuteMethodEverySeconds(seconds * 1000)
            {
                Method = method
            };

            return timer;
        }
    }

    public interface IExtendedTimer
    {
        bool RunOnUnityThread
        {
            get;
            set;
        }
    }
}