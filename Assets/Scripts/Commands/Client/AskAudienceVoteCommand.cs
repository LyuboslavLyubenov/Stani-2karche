using System.Collections.Generic;

public class AskAudienceVoteCommand : IOneTimeExecuteCommand
{
    public delegate void OnReceivedVote(int connectionId,string answer);

    OnReceivedVote onReceivedVote;

    public bool FinishedExecution
    {
        get;
        private set;
    }

    public AskAudienceVoteCommand(OnReceivedVote onReceivedVote)
    {
        if (onReceivedVote == null)
        {
            throw new System.ArgumentNullException("onReceivedVote");
        }
            
        this.onReceivedVote = onReceivedVote;
    }

    public void Execute(Dictionary<string, string> commandsOptionsValues)
    {
        var connectionId = int.Parse(commandsOptionsValues["ConnectionId"]);
        var answer = commandsOptionsValues["Answer"];
        onReceivedVote(connectionId, answer);

        FinishedExecution = true;
    }
}
