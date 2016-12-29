namespace Assets.Scripts.Commands.Client
{
    using System;
    using System.Collections.Generic;

    using UnityEngine;

    using Interfaces;
    using Notifications;

    public class AnswerTimeoutCommand : INetworkManagerCommand
    {
        private const string AnsweTimeoutMessage = "Времето за отговор изтече";

        private GameObject questionPanelUI;

        private NotificationsServiceController notificationService;

        public AnswerTimeoutCommand(GameObject questionPanelUI, NotificationsServiceController notificationService)
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
            this.questionPanelUI.SetActive(false);
            this.notificationService.AddNotification(Color.blue, AnsweTimeoutMessage);
        }
    }
}