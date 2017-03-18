namespace EventArgs
{

    using System;
    using System.Collections.Generic;

    public class VoteEventArgs : System.EventArgs
    {
        public VoteEventArgs(Dictionary<string, int> answersVotes)
        {
            if (answersVotes == null)
            {
                throw new ArgumentNullException("answersVotes");
            }

            this.AnswersVotes = answersVotes;
        }

        public Dictionary<string,int> AnswersVotes
        {
            get;
            private set;
        }
    }
}