namespace Utils
{

    using System;
    using System.Timers;

    public class Timer_ExecuteMethodAfterTime : ExtendedTimer, IExtendedTimer
    {
        public Action Method
        {
            get; set;
        }

        public bool AutoDispose
        {
            get; set;
        }

        public bool RunOnUnityThread
        {
            get; set;
        }

        public Timer_ExecuteMethodAfterTime(double interval)
        {
            this.Interval = interval;
            base.AutoReset = false;
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

            base.Stop();

            if (this.AutoDispose)
            {
                base.Dispose();
            }
        }
    }

}