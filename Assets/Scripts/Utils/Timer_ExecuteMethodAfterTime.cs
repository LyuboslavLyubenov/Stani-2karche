namespace Assets.Scripts.Utils
{

    using System;
    using System.Timers;

    public class Timer_ExecuteMethodAfterTime : Timer
    {
        public Action Method
        {
            get; set;
        }

        public bool AutoDispose
        {
            get; set;
        }

        public Timer_ExecuteMethodAfterTime(double interval)
            : base(interval)
        {
            base.AutoReset = false;
            base.Elapsed += this.OnElapsed;
        }

        void OnElapsed(object sender, ElapsedEventArgs args)
        {
            ThreadUtils.Instance.RunOnMainThread(this.Method);
            base.Stop();

            if (this.AutoDispose)
            {
                base.Dispose();
            }
        }
    }

}