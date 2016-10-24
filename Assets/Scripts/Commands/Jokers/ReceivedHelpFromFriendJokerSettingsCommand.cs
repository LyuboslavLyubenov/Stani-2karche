using System.Linq;
using System;
using UnityEngine;
using System.Collections.Generic;
using System.Timers;

public class ReceivedHelpFromFriendJokerSettingsCommand : IOneTimeExecuteCommand
{
    ClientNetworkManager networkManager;

    GameObject loadingUI;
    GameObject waitingToAnswerUI;
    GameObject friendAnswerUI;

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

    public ReceivedHelpFromFriendJokerSettingsCommand(ClientNetworkManager networkManager, GameObject loadingUI, GameObject waitingToAnswerUI)
    {
        if (networkManager == null)
        {
            throw new ArgumentNullException("networkManager");
        }
            
        if (loadingUI == null)
        {
            throw new ArgumentNullException("loadingUI");
        }
            
        if (waitingToAnswerUI == null)
        {
            throw new ArgumentNullException("waitingToAnswerUI");
        }

        this.networkManager = networkManager;
        this.loadingUI = loadingUI;
        this.waitingToAnswerUI = waitingToAnswerUI;
    }

    public void Execute(Dictionary<string, string> commandsOptionsValues)
    {
        var timeToAnswerInSeconds = int.Parse(commandsOptionsValues["TimeToAnswerInSeconds"]);
        
        loadingUI.SetActive(false);
        waitingToAnswerUI.SetActive(true);

        waitingToAnswerUI.GetComponent<DisableAfterDelay>().DelayInSeconds = timeToAnswerInSeconds;

        FinishedExecution = true;

        if (OnFinishedExecution != null)
        {
            OnFinishedExecution(this, EventArgs.Empty);
        }
    }
}