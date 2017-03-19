namespace Commands.Client
{

    using System;
    using System.Collections.Generic;

    using Interfaces.Network.NetworkManager;

    using Notifications;

    using UnityEngine;

    public class AnswerTimeoutCommand : INetworkManagerCommand
    {
        private const string AnsweTimeoutMessage = "Времето за отговор изтече";

        private GameObject questionPanelUI;

        private NotificationsController notification;

        public AnswerTimeoutCommand(GameObject questionPanelUI, NotificationsController notification)
        {
            if (notification == null)
            {
                throw new ArgumentNullException("notification");
            }
            
            if (questionPanelUI == null)
            {
                throw new ArgumentNullException("questionPanelUI");
            }
            
            this.questionPanelUI = questionPanelUI;
            this.notification = notification;
        }

        public void Execute(Dictionary<string, string> commandsOptionsValues)
        {
            this.questionPanelUI.SetActive(false);
            this.notification.AddNotification(Color.blue, AnsweTimeoutMessage);
        }
    }
}