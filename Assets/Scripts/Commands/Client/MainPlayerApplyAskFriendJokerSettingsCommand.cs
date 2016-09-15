using System.Linq;
using System;
using UnityEngine;
using System.Collections.Generic;

public class MainPlayerApplyAskFriendJokerSettingsCommand : IOneTimeExecuteCommand
{
    DisableAfterDelay disableAfterDelay;

    Action onAppliedSettings;

    public bool FinishedExecution
    {
        get;
        private set;
    }

    public MainPlayerApplyAskFriendJokerSettingsCommand(DisableAfterDelay disableAfterDelay, Action onAppliedSettings)
    {
        if (disableAfterDelay == null)
        {
            throw new ArgumentNullException("disableAfterDelay");
        }

        if (onAppliedSettings == null)
        {
            throw new ArgumentNullException("onAppliedSettings");
        }
            
        this.disableAfterDelay = disableAfterDelay;
        this.onAppliedSettings = onAppliedSettings;
    }

    public void Execute(Dictionary<string, string> commandsOptionsValues)
    {
        var timeToAnswerInSeconds = int.Parse(commandsOptionsValues["TimeToAnswerInSeconds"]);
        disableAfterDelay.DelayInSeconds = timeToAnswerInSeconds;
        onAppliedSettings();
        FinishedExecution = true;
    }
}