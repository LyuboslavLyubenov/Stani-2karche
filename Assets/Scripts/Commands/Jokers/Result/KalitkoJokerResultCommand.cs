using IOneTimeExecuteCommand = Interfaces.Network.NetworkManager.IOneTimeExecuteCommand;

namespace Assets.Scripts.Commands.Jokers.Result
{

    using System;
    using System.Collections.Generic;

    using Assets.Scripts.Interfaces.Controllers;

    using UnityEngine;

    public class KalitkoJokerResultCommand : IOneTimeExecuteCommand
    {
        private readonly IKalitkoJokerUIController kalitkoJokerContainerUiController;

        private readonly GameObject kalitkoJokerUi;

        public EventHandler OnFinishedExecution
        {
            get; set;
        }

        public bool FinishedExecution
        {
            get; private set;
        }

        public KalitkoJokerResultCommand(IKalitkoJokerUIController kalitkoJokerContainerUiController, GameObject kalitkoJokerUI)
        {
            if (kalitkoJokerContainerUiController == null)
            {
                throw new ArgumentNullException("kalitkoJokerContainerUiController");
            }

            if (kalitkoJokerUI == null)
            {
                throw new ArgumentNullException("kalitkoJokerUI");
            }

            this.kalitkoJokerContainerUiController = kalitkoJokerContainerUiController;
            this.kalitkoJokerUi = kalitkoJokerUI;
        }

        public void Execute(Dictionary<string, string> commandsOptionsValues)
        {
            this.kalitkoJokerUi.SetActive(true);

            if (!commandsOptionsValues.ContainsKey("Answer"))
            {
                this.kalitkoJokerContainerUiController.ShowNothing();
            }
            else
            {
                this.kalitkoJokerContainerUiController.ShowAnswer(commandsOptionsValues["Answer"]);
            }

            this.FinishedExecution = true;
            this.OnFinishedExecution(this, EventArgs.Empty);
        }
    }
}
