using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class OnAnswerTimeoutCommand : INetworkManagerCommand
{
    const string AnsweTimeoutMessage = "Времето за отговор изтече";

    GameObject questionPanelUI;

    GameState currentGameState;

    NotificationsServiceController notificationService;

    public OnAnswerTimeoutCommand(GameObject questionPanelUI, GameState currentGameState, NotificationsServiceController notificationService)
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
        this.currentGameState = currentGameState; 
        this.notificationService = notificationService;
    }

    public void Execute(Dictionary<string, string> commandsOptionsValues)
    {
        if (currentGameState != GameState.Idle)
        {
            throw new Exception("Cant use this command now");
        }

        questionPanelUI.SetActive(false);
        notificationService.AddNotification(Color.blue, AnsweTimeoutMessage);
    }
}
