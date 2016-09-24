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

    public ReceivedHelpFromFriendJokerSettingsCommand(ClientNetworkManager networkManager, GameObject loadingUI, GameObject waitingToAnswerUI, GameObject friendAnswerUI)
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
            
        if (friendAnswerUI == null)
        {
            throw new ArgumentNullException("friendAnswerUI");
        }

        this.networkManager = networkManager;
        this.loadingUI = loadingUI;
        this.waitingToAnswerUI = waitingToAnswerUI;
        this.friendAnswerUI = friendAnswerUI;
    }

    public void Execute(Dictionary<string, string> commandsOptionsValues)
    {
        var timeToAnswerInSeconds = int.Parse(commandsOptionsValues["TimeToAnswerInSeconds"]);
        
        loadingUI.SetActive(false);
        waitingToAnswerUI.SetActive(true);

        waitingToAnswerUI.GetComponent<DisableAfterDelay>().DelayInSeconds = timeToAnswerInSeconds;

        networkManager.CommandsManager.AddCommand("HelpFromFriendJokerResponse", new ReceivedHelpFromFriendResponseCommand(waitingToAnswerUI, friendAnswerUI));

        FinishedExecution = true;

        if (OnFinishedExecution != null)
        {
            OnFinishedExecution(this, EventArgs.Empty);
        }
    }
}