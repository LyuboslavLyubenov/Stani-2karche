using System;

public class AskPlayerResponseCommand : IOneTimeExecuteCommand
{
    public delegate void OnReceivedAnswer(string username,string answer);

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

    OnReceivedAnswer onReceivedAnswer;

    public AskPlayerResponseCommand(OnReceivedAnswer onReceivedAnswer)
    {
        if (onReceivedAnswer == null)
        {
            throw new ArgumentNullException("onReceivedAnswer");
        }
            
        this.onReceivedAnswer = onReceivedAnswer;

        FinishedExecution = false;
    }

    public void Execute(System.Collections.Generic.Dictionary<string, string> commandsOptionsValues)
    {
        var username = commandsOptionsValues["Username"];
        var answer = commandsOptionsValues["Answer"];

        onReceivedAnswer(username, answer);

        FinishedExecution = true;

        if (OnFinishedExecution != null)
        {
            OnFinishedExecution(this, EventArgs.Empty);
        }
    }
}