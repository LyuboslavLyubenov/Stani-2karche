using IJokerElectionUIController = Controllers.EveryBodyVsTheTeacher.Jokers.Election.IJokerElectionUIController;
using INetworkManagerCommand = Interfaces.Network.NetworkManager.INetworkManagerCommand;

namespace Assets.Scripts.Commands.Jokers.Election
{

    using System;
    using System.Collections.Generic;

    public class VotedAgainstCommand : INetworkManagerCommand
    {
        private readonly IJokerElectionUIController electionUIController;

        public VotedAgainstCommand(IJokerElectionUIController electionUIController)
        {
            if (electionUIController == null)
            {
                throw new ArgumentNullException("electionUIController");
            }

            this.electionUIController = electionUIController;
        }

        public void Execute(Dictionary<string, string> commandsOptionsValues)
        {
            this.electionUIController.AddThumbsDown();
        }
    }

}