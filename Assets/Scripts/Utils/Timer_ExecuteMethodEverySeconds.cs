﻿namespace Utils
{

    using System;
    using System.Timers;

    public class Timer_ExecuteMethodEverySeconds : Timer, IExtendedTimer
    {
        public Action Method
        {
            get; set;
        }

        public bool RunOnUnityThread
        {
            get; set;
        }

        public Timer_ExecuteMethodEverySeconds(double interval)
            : base(interval)
        {
            base.AutoReset = true;
            base.Elapsed += this.OnElapsed;
        }

        private void OnElapsed(object sender, ElapsedEventArgs args)
        {
            if (this.RunOnUnityThread)
            {
                ThreadUtils.Instance.RunOnMainThread(this.Method);
            }
            else
            {
                this.Method();
            }
        }
    }
}