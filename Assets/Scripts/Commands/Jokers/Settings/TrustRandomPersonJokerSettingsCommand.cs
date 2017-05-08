using INetworkManagerCommand = Interfaces.Network.NetworkManager.INetworkManagerCommand;

namespace Scripts.Commands.Jokers.Settings
{

    using System;
    using System.Collections.Generic;

    using Assets.Scripts.Interfaces.Controllers;

    using Extensions;

    using UnityEngine;

    public class TrustRandomPersonJokerSettingsCommand : INetworkManagerCommand
    {
        private readonly ISecondsRemainingUIController secondsRemainingUIController;
        private readonly GameObject secondsRemainingUI;

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
            var timeToAnswerInSeconds = commandsOptionsValues["TimeToAnswerInSeconds"]
                .ConvertTo<int>();

            this.secondsRemainingUI.SetActive(true);
            this.secondsRemainingUIController.InvervalInSeconds = timeToAnswerInSeconds;
            this.secondsRemainingUIController.StartTimer();
        }
    }
}
