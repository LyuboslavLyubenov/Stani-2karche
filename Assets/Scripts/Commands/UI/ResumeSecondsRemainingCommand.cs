using INetworkManagerCommand = Interfaces.Network.NetworkManager.INetworkManagerCommand;

namespace Assets.Scripts.Commands.UI
{

    using System;
    using System.Collections.Generic;

    using Assets.Scripts.Interfaces.Controllers;

    public class ResumeSecondsRemainingCommand : INetworkManagerCommand
    {
        private readonly ISecondsRemainingUIController secondsRemainingUIController;

        public ResumeSecondsRemainingCommand(ISecondsRemainingUIController secondsRemainingUIController)
        {
            if (secondsRemainingUIController == null)
            {
                throw new ArgumentNullException("secondsRemainingUIController");
            }

            this.secondsRemainingUIController = secondsRemainingUIController;
        }

        public void Execute(Dictionary<string, string> commandsOptionsValues)
        {
            this.secondsRemainingUIController.Paused = false;
        }
    }
}
