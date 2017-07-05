namespace Utils
{

    using System;

    public class TimerUtils
    {
        private TimerUtils()
        {
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