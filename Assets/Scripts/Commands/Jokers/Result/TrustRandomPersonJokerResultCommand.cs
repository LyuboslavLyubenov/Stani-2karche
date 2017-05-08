using IOneTimeExecuteCommand = Interfaces.Network.NetworkManager.IOneTimeExecuteCommand;

namespace Assets.Scripts.Commands.Jokers.Result
{
    using System;
    using System.Collections.Generic;

    using Assets.Scripts.Interfaces.Controllers;

    using UnityEngine;

    public class TrustRandomPersonJokerResultCommand : IOneTimeExecuteCommand
    {
        private readonly GameObject secondsRemainingUI;
        private readonly GameObject notReceivedAnswerUI;
        private readonly GameObject playerAnswerUI;
        private readonly IPlayerAnswerUIController playerAnswerUIController;

        public EventHandler OnFinishedExecution
        {
            get; set;
        }

        public bool FinishedExecution
        {
            get; private set;
        }

        public TrustRandomPersonJokerResultCommand(
            GameObject secondsRemainingUI, 
            GameObject notReceivedAnswerUI,
            GameObject playerAnswerUI, 
            IPlayerAnswerUIController playerAnswerUIController)
        {
            if (secondsRemainingUI == null)
            {
                throw new ArgumentNullException("secondsRemainingUI");
            }

            if (notReceivedAnswerUI == null)
            {
                throw new ArgumentNullException("notReceivedAnswerUI");
            }

            if (playerAnswerUI == null)
            {
                throw new ArgumentNullException("playerAnswerUI");
            }

            if (playerAnswerUIController == null)
            {
                throw new ArgumentNullException("playerAnswerUIController");
            }

            this.secondsRemainingUI = secondsRemainingUI;
            this.notReceivedAnswerUI = notReceivedAnswerUI;
            this.playerAnswerUI = playerAnswerUI;
            this.playerAnswerUIController = playerAnswerUIController;
        }

        public void Execute(Dictionary<string, string> commandsOptionsValues)
        {
            this.secondsRemainingUI.SetActive(false);

            if (commandsOptionsValues.ContainsKey("Username") && commandsOptionsValues.ContainsKey("Answer"))
            {
                var username = commandsOptionsValues["Username"];
                var answer = commandsOptionsValues["Answer"];

                this.playerAnswerUI.SetActive(true);
                this.playerAnswerUIController.SetResponse(username, answer);
            }
            else
            {
                this.notReceivedAnswerUI.SetActive(true);
            }

            this.FinishedExecution = true;

            if (this.OnFinishedExecution != null)
            {
                this.OnFinishedExecution(this, EventArgs.Empty);
            }
        }
    }
}