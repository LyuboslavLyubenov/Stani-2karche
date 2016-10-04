using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

public class ReceivedSurrenderBasicExamOneTimeCommand : IOneTimeExecuteCommand
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

    MainPlayerData mainPlayerData;

    Action onSurrender;

    public ReceivedSurrenderBasicExamOneTimeCommand(MainPlayerData mainPlayerData, Action onSurrender)
    {
        if (mainPlayerData == null)
        {
            throw new ArgumentNullException("mainPlayerData");
        }
            
        if (onSurrender == null)
        {
            throw new ArgumentNullException("onSurrender");
        }

        this.mainPlayerData = mainPlayerData;    
        this.onSurrender = onSurrender;
    }

    public void Execute(Dictionary<string, string> commandsOptionsValues)
    {
        var connectionId = int.Parse(commandsOptionsValues["ConnectionId"]);

        if (!mainPlayerData.IsConnected || mainPlayerData.ConnectionId != connectionId)
        {
            return;   
        }

        onSurrender();

        FinishedExecution = true;

        if (OnFinishedExecution != null)
        {
            OnFinishedExecution(this, EventArgs.Empty);    
        }
    }
}