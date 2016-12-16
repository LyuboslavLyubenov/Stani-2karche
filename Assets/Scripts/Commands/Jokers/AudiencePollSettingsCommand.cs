using UnityEngine;
using System;
using System.Collections.Generic;
using System.Timers;
using System.Linq;

public class AudiencePollSettingsCommand : IOneTimeExecuteCommand
{
    public delegate void OnReceivedSettings(int timeToAnswerInSeconds);

    OnReceivedSettings onReceivedSettings;

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

    public AudiencePollSettingsCommand(OnReceivedSettings onReceivedSettings)
    {
        if (onReceivedSettings == null)
        {
            throw new ArgumentNullException("onReceivedSettings");
        }
            
        this.onReceivedSettings = onReceivedSettings;
    }

    public void Execute(Dictionary<string, string> commandsOptionsValues)
    {
        var timeToAnswer = int.Parse(commandsOptionsValues["TimeToAnswerInSeconds"]);
        onReceivedSettings(timeToAnswer);

        if (OnFinishedExecution != null)
        {
            OnFinishedExecution(this, EventArgs.Empty);
        }
    }
}