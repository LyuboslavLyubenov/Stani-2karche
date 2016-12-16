using System;
using System.Collections.Generic;

public class HelpFromFriendJokerSettingsCommand : IOneTimeExecuteCommand
{
    public delegate void OnReceivedSettings(int timeToAnswerInSeconds);

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

    OnReceivedSettings onReceivedSettings;

    public HelpFromFriendJokerSettingsCommand(OnReceivedSettings onReceivedSettings)
    {
        if (onReceivedSettings == null)
        {
            throw new ArgumentNullException("onReceivedSettings");
        }

        this.onReceivedSettings = onReceivedSettings;
    }

    public void Execute(Dictionary<string, string> commandsOptionsValues)
    {
        var timeToAnswerInSeconds = int.Parse(commandsOptionsValues["TimeToAnswerInSeconds"]);

        onReceivedSettings(timeToAnswerInSeconds);

        FinishedExecution = true;

        if (OnFinishedExecution != null)
        {
            OnFinishedExecution(this, EventArgs.Empty);
        }
    }
}