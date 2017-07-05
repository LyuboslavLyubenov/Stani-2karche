namespace Utils
{
    using System;
    using System.Timers;

    public class Timer_ExecuteMethodWhen : ExtendedTimer, IExtendedTimer
    {
        private readonly Func<bool> condition;
        private readonly Action method;

        public bool RunOnUnityThread
        {
            get; set;
        }

        public Timer_ExecuteMethodWhen(Func<bool> condition, Action method)
        {
            if (condition == null)
            {
                throw new ArgumentNullException("condition");
            }

            if (method == null)
            {
                throw new ArgumentNullException("method");
            }

            this.condition = condition;
            this.method = method;

            this.AutoReset = true;
            this.Interval = 300;
            this.Elapsed += this.OnElapsed;
        }

        private void OnConditionMet()
        {
            this.method();
            this.Stop();
            this.Dispose();
        }

        private void OnElapsed(object sender, ElapsedEventArgs args)
        {
            if (this.condition() == false)
            {
                return;
            }
            
            if (this.RunOnUnityThread)
            {
                ThreadUtils.Instance.RunOnMainThread(this.OnConditionMet);
            }
            else
            {
                this.OnConditionMet();
            }
        }

        public new void Pause()
        {
            throw new InvalidOperationException();
        }

        public new void Resume()
        {
            throw new InvalidOperationException();
        }
    }
}