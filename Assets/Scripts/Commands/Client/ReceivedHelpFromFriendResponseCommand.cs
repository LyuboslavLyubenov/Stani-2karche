using System;
using UnityEngine;

public class ReceivedHelpFromFriendResponseCommand : IOneTimeExecuteCommand
{
    FriendAnswerUIController friendAnswerUIController;

    GameObject friendAnswerUI;
    GameObject waitingToAnswerUI;

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

    public ReceivedHelpFromFriendResponseCommand(GameObject waitingToAnswerUI, GameObject friendAnswerUI)
    {
        this.waitingToAnswerUI = waitingToAnswerUI;
        this.friendAnswerUI = friendAnswerUI;
        this.friendAnswerUIController = friendAnswerUI.GetComponent<FriendAnswerUIController>();

        if (friendAnswerUIController == null)
        {
            throw new Exception("Friend answer ui doenst have FriendAnswerUIController component");
        }

        FinishedExecution = false;
    }

    public void Execute(System.Collections.Generic.Dictionary<string, string> commandsOptionsValues)
    {
        var username = commandsOptionsValues["Username"];
        var answer = commandsOptionsValues["Answer"];

        waitingToAnswerUI.SetActive(false);
        friendAnswerUI.SetActive(true);
        friendAnswerUIController.SetResponse(username, answer);

        FinishedExecution = true;

        if (OnFinishedExecution != null)
        {
            OnFinishedExecution(this, EventArgs.Empty);
        }
    }
}
