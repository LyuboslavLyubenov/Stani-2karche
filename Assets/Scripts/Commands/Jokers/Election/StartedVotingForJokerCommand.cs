using IJokerElectionUIController = Assets.Scripts.Interfaces.Controllers.IJokerElectionUIController;
using INetworkManagerCommand = Interfaces.Network.NetworkManager.INetworkManagerCommand;

namespace Assets.Scripts.Commands.Jokers.Election
{
    using System;
    using System.Collections.Generic;

    using Assets.Scripts.Interfaces;

    using UnityEngine;

    public class StartedVotingForJokerCommand : INetworkManagerCommand
    {
        private readonly IJokerElectionUIController jokerElectionUIController;
        private readonly GameObject jokerElectionUI;
        private readonly IJoker joker;

        public StartedVotingForJokerCommand(
            IJokerElectionUIController jokerElectionUIController, 
            GameObject jokerElectionUI,
            IJoker joker)
        {
            if (jokerElectionUIController == null)
            {
                throw new ArgumentNullException("jokerElectionUIController");
            }

            if (jokerElectionUI == null)
            {
                throw new ArgumentNullException("jokerElectionUI");
            }

            if (joker == null)
            {
                throw new ArgumentNullException("joker");
            }

            this.jokerElectionUIController = jokerElectionUIController;
            this.jokerElectionUI = jokerElectionUI;
            this.joker = joker;
        }

        public void Execute(Dictionary<string, string> commandsOptionsValues)
        {
            this.jokerElectionUI.SetActive(true);
            this.jokerElectionUIController.SetJokerImage(this.joker.Image);
        }
    }
}