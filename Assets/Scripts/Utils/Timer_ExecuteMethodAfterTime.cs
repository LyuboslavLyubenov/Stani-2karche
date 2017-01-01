namespace Assets.Scripts.Utils
{
    using System;
    using System.Timers;

    public class Timer_ExecuteMethodAfterTime : Timer, IExtendedTimer
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
            : base(interval)
        {
            base.AutoReset = false;
            base.Elapsed += this.OnElapsed;
        }

        private void OnElapsed(object sender, ElapsedEventArgs args)
        {
            if (RunOnUnityThread)
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