namespace Assets.Scripts.EventArgs
{

    using System;

    using EventArgs = System.EventArgs;

    public class AskPlayerResponseEventArgs : EventArgs
    {
        public string Username
        {
            get;
            private set;
        }

        public string Answer
        {
            get;
            private set;
        }

        public AskPlayerResponseEventArgs(string username, string answer)
        {
            if (username == null)
            {
                throw new ArgumentNullException("username");
            }
            
            if (answer == null)
            {
                throw new ArgumentNullException("answer");
            }

            this.Username = username;
            this.Answer = answer;
        }
    }

}