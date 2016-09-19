using System.Collections.Generic;
using System;

public class AskAudienceReceivedVoteCommandOneTime : IOneTimeExecuteCommand
{
    public delegate void OnReceivedVote(int connectionId,string answer);

    OnReceivedVote onReceivedVote;

    public bool FinishedExecution
    {
        get;
        private set;
    }

    public EventHandler OnFinishedExecution
    {
        get;
        set;
    }

    public AskAudienceReceivedVoteCommandOneTime(OnReceivedVote onReceivedVote)
    {
        if (onReceivedVote == null)
        {
            throw new ArgumentNullException("onReceivedVote");
        }
            
        this.onReceivedVote = onReceivedVote;
    }

    public void Execute(Dictionary<string, string> commandsOptionsValues)
    {
        var connectionId = int.Parse(commandsOptionsValues["ClientConnectionId"]);
        var answer = commandsOptionsValues["Answer"];
        onReceivedVote(connectionId, answer);

        FinishedExecution = true;

        if (OnFinishedExecution != null)
        {
            OnFinishedExecution(this, EventArgs.Empty);
        }
    }
}
