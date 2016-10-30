using System;

//Mediator
using System.Collections.Generic;

public class DummyOneTimeCommand : IOneTimeExecuteCommand
{
    public EventHandler OnFinishedExecution
    {
        get;
        set;
    }

    public bool FinishedExecution
    {
        get;
        private set;
    }

    public void Execute(Dictionary<string, string> commandsOptionsValues)
    {
        if (OnFinishedExecution != null)
        {
            OnFinishedExecution(this, EventArgs.Empty);
        }

        FinishedExecution = true;
    }
}