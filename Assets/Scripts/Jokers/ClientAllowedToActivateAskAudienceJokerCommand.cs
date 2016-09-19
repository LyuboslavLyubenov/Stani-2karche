using UnityEngine;
using System;
using System.Collections.Generic;
using System.Timers;
using System.Linq;

public class ClientAllowedToActivateAskAudienceJokerCommand : IOneTimeExecuteCommand
{
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

    public void Execute(Dictionary<string, string> commandsOptionsValues)
    {
        if (OnFinishedExecution != null)
        {
            OnFinishedExecution(this, EventArgs.Empty);
        }
    }
}