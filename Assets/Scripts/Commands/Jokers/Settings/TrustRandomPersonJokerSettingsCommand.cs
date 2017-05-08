using IOneTimeExecuteCommand = Interfaces.Network.NetworkManager.IOneTimeExecuteCommand;
using StringExtensions = Extensions.StringExtensions;

namespace Assets.Scripts.Commands.Jokers.Settings
{

    using System;
    using System.Collections.Generic;

    using Assets.Scripts.Interfaces.Controllers;

    using UnityEngine;

    public class TrustRandomPersonJokerSettingsCommand : IOneTimeExecuteCommand
    {
        private readonly ISecondsRemainingUIController secondsRemainingUIController;
        private readonly GameObject secondsRemainingUI;
        
        public EventHandler OnFinishedExecution
        {
            get; set;
        }

        public bool FinishedExecution
        {
            get; private set;
        }

        public TrustRandomPersonJokerSettingsCommand(ISecondsRemainingUIController secondsRemainingUIController, GameObject secondsRemainingUI)
        {
            if (secondsRemainingUIController == null)
            {
                throw new ArgumentNullException("secondsRemainingUIController");
            }

            if (secondsRemainingUI == null)
            {
                throw new ArgumentNullException("secondsRemainingUI");
            }

            this.secondsRemainingUIController = secondsRemainingUIController;
            this.secondsRemainingUI = secondsRemainingUI;
        }

        public void Execute(Dictionary<string, string> commandsOptionsValues)
        {
            var timeToAnswerInSeconds = StringExtensions.ConvertTo<int>(commandsOptionsValues["TimeToAnswerInSeconds"]);

            this.secondsRemainingUI.SetActive(true);
            this.secondsRemainingUIController.InvervalInSeconds = timeToAnswerInSeconds;
            this.secondsRemainingUIController.StartTimer();

            this.FinishedExecution = true;

            if (this.OnFinishedExecution != null)
            {
                this.OnFinishedExecution(this, EventArgs.Empty);
            }
        }
    }
}
