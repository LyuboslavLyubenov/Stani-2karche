namespace Utils
{

    using System;
    using System.Timers;

    public class ExtendedTimer : Timer
    {
        private DateTime endTime;

        public double RemainingTimeInMiliseconds
        {
            get
            {
                return (this.endTime - DateTime.Now).TotalMilliseconds;
            }
        }
        
        public ExtendedTimer()
        {
            this.Elapsed += this.OnElapsed;
        }

        private void OnElapsed(object sender, ElapsedEventArgs args)
        {
            if (this.AutoReset)
            {
                this.endTime = DateTime.Now.AddMilliseconds(this.Interval);
            }
        }

        public new void Start()
        {
            this.endTime = DateTime.Now.AddMilliseconds(this.Interval);
            base.Start();
        }

        protected override void Dispose(bool disposing)
        {
            this.Elapsed -= this.OnElapsed;
            base.Dispose(disposing);
        }
    }
}