using System;

public class ReceivedAskAFriendResponseCommand : IOneTimeExecuteCommand
{
    public delegate void OnReceivedResponde(string username,string answer);

    OnReceivedResponde onReceivedResponde;

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

    public ReceivedAskAFriendResponseCommand(OnReceivedResponde onReceivedResponde)
    {
        if (onReceivedResponde == null)
        {
            throw new ArgumentNullException("onReceivedResponde");
        }
            
        this.onReceivedResponde = onReceivedResponde;
        FinishedExecution = false;
    }

    public void Execute(System.Collections.Generic.Dictionary<string, string> commandsOptionsValues)
    {
        var username = commandsOptionsValues["Username"];
        var answer = commandsOptionsValues["Answer"];

        onReceivedResponde(username, answer);
        FinishedExecution = true;

        if (OnFinishedExecution != null)
        {
            OnFinishedExecution(this, EventArgs.Empty);
        }
    }
}
