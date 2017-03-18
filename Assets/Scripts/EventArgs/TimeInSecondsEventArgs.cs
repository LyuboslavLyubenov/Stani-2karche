namespace EventArgs
{

    using System;

    public class TimeInSecondsEventArgs : System.EventArgs
    {
        public TimeInSecondsEventArgs(int seconds)
        {
            if (seconds < 0)
            {
                throw new ArgumentOutOfRangeException("seconds");
            }

            this.Seconds = seconds;
        }

        public int Seconds
        {
            get;
            private set;
        }
    }

}
