using UnityEngine;
using System.Collections.Generic;
using System;

public class ReceivedAnswerTimeoutCommand : INetworkManagerCommand
{
    const string AnsweTimeoutMessage = "Времето за отговор изтече";

    GameObject questionPanelUI;

    NotificationsServiceController notificationService;

    public ReceivedAnswerTimeoutCommand(GameObject questionPanelUI, NotificationsServiceController notificationService)
    {
        if (notificationService == null)
        {
            throw new ArgumentNullException("notificationService");
        }
            
        if (questionPanelUI == null)
        {
            throw new ArgumentNullException("questionPanelUI");
        }
            
        this.questionPanelUI = questionPanelUI;
        this.notificationService = notificationService;
    }

    public void Execute(Dictionary<string, string> commandsOptionsValues)
    {
        questionPanelUI.SetActive(false);
        notificationService.AddNotification(Color.blue, AnsweTimeoutMessage);
    }
}
