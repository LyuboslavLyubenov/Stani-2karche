using System.Collections.Generic;
using System;

public class AudienceVoteEventArgs : System.EventArgs
{
    public AudienceVoteEventArgs(Dictionary<string, int> answersVotes)
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

