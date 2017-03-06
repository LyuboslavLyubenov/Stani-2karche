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

        private NotificationsesController notificationse;

        public AnswerTimeoutCommand(GameObject questionPanelUI, NotificationsesController notificationse)
        {
            if (notificationse == null)
            {
                throw new ArgumentNullException("notificationse");
            }
            
            if (questionPanelUI == null)
            {
                throw new ArgumentNullException("questionPanelUI");
            }
            
            this.questionPanelUI = questionPanelUI;
            this.notificationse = notificationse;
        }

        public void Execute(Dictionary<string, string> commandsOptionsValues)
        {
            this.questionPanelUI.SetActive(false);
            this.notificationse.AddNotification(Color.blue, AnsweTimeoutMessage);
        }
    }
}