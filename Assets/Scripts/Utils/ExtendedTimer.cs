namespace Utils
{

    using System;
    using System.Timers;

    public class ExtendedTimer : Timer
    {
        private DateTime endTime;
        private DateTime pauseTime;
        private bool paused;

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
            if (this.paused)
            {
                throw new InvalidOperationException();
            }
            
            this.endTime = DateTime.Now.AddMilliseconds(this.Interval);
            base.Start();
        }

        public void Pause()
        {
            if (this.RemainingTimeInMiliseconds <= 0)
            {
                throw new InvalidOperationException();
            }
            
            this.pauseTime = DateTime.Now;
            this.paused = true;
            this.Stop();
        }

        public void Resume()
        {
            if (this.Enabled)
            {
                throw new InvalidOperationException();
            }

            this.Interval = (this.endTime - this.pauseTime).TotalMilliseconds;
            this.paused = false;
            base.Start();
        }
        
        protected override void Dispose(bool disposing)
        {
            this.Elapsed -= this.OnElapsed;
            base.Dispose(disposing);
        }
    }
}