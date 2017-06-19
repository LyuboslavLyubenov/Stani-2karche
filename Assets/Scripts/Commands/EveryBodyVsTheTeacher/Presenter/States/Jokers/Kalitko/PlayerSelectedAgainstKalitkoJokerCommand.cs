using IJokerElectionUIController = Assets.Scripts.Interfaces.Controllers.IJokerElectionUIController;
using INetworkManagerCommand = Interfaces.Network.NetworkManager.INetworkManagerCommand;

namespace Assets.Scripts.Commands.EveryBodyVsTheTeacher.Presenter.States.Jokers.Kalitko
{
    using System;
    using System.Collections.Generic;

    public class PlayerSelectedAgainstKalitkoJokerCommand : INetworkManagerCommand
    {
        private readonly IJokerElectionUIController jokerElectionUiController;

        public PlayerSelectedAgainstKalitkoJokerCommand(IJokerElectionUIController jokerElectionUiController)
        {
            if (jokerElectionUiController == null)
            {
                throw new ArgumentNullException("jokerElectionUiController");
            }

            this.jokerElectionUiController = jokerElectionUiController;
        }

        public void Execute(Dictionary<string, string> commandsOptionsValues)
        {
            this.jokerElectionUiController.AddThumbsDown();
        }
    }
}
