namespace Assets.Scripts.Utils
{

    using System;
    using System.Timers;

    public class Timer_ExecuteMethodEverySeconds : Timer
    {
        public Action Method
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
            ThreadUtils.Instance.RunOnMainThread(this.Method);
        }
    }

}