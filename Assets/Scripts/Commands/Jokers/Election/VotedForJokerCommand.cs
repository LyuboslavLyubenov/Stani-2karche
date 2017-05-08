using IJokerElectionUIController = Assets.Scripts.Interfaces.Controllers.IJokerElectionUIController;
using INetworkManagerCommand = Interfaces.Network.NetworkManager.INetworkManagerCommand;

namespace Assets.Scripts.Commands.Jokers.Election
{

    using System;
    using System.Collections.Generic;

    public class VotedForJokerCommand : INetworkManagerCommand
    {
        private readonly IJokerElectionUIController jokerElectionUIController;

        public VotedForJokerCommand(IJokerElectionUIController jokerElectionUIController)
        {
            if (jokerElectionUIController == null)
            {
                throw new ArgumentNullException("jokerElectionUIController");
            }

            this.jokerElectionUIController = jokerElectionUIController;
        }

        public void Execute(Dictionary<string, string> commandsOptionsValues)
        {
            this.jokerElectionUIController.AddThumbsUp();
        }
    }
}