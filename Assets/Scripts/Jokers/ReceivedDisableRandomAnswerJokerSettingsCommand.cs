using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Timers;

public class ReceivedDisableRandomAnswerJokerSettingsCommand : IOneTimeExecuteCommand
{
    public delegate void OnReceivedJokerSettings(int answersToDisableCount);

    OnReceivedJokerSettings onReceivedJokerSettings;

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

    public ReceivedDisableRandomAnswerJokerSettingsCommand(OnReceivedJokerSettings onReceivedJokerSettings)
    {
        if (onReceivedJokerSettings == null)
        {
            throw new ArgumentNullException("onReceivedJokerSettings");
        }
            
        this.onReceivedJokerSettings = onReceivedJokerSettings;
    }

    public void Execute(Dictionary<string, string> commandsOptionsValues)
    {
        var answersToDisableCount = int.Parse(commandsOptionsValues["AnswersToDisableCount"]);
        onReceivedJokerSettings(answersToDisableCount);
        FinishedExecution = true;

        if (OnFinishedExecution != null)
        {
            OnFinishedExecution(this, EventArgs.Empty);
        }
    }
}