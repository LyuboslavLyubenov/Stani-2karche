﻿namespace Utils
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
            var threadUtils = ThreadUtils.Instance;//Make sure threadutils is initialized

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
            var threadUtils = ThreadUtils.Instance;//Make sure threadutils is initialized

            var timer = new Timer_ExecuteMethodEverySeconds(seconds * 1000)
            {
                Method = method
            };

            return timer;
        }
    }

}