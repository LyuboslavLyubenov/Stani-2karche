using System;

namespace Assets.Scripts.EventArgs
{

    using EventArgs = System.EventArgs;

    public class JokerSettingsEventArgs : EventArgs
    {
        public int TimeToAnswerInSeconds
        { 
            get; 
            set; 
        }

        public JokerSettingsEventArgs(int timeToAnswerInSeconds)
        {
            if (timeToAnswerInSeconds < 0)
            {
                throw new ArgumentOutOfRangeException("timeToAnswerInSeconds");
            }

            this.TimeToAnswerInSeconds = timeToAnswerInSeconds;
        }
    }

}