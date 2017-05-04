using IJokerElectionUIController = Controllers.EveryBodyVsTheTeacher.Jokers.Election.IJokerElectionUIController;
using INetworkManagerCommand = Interfaces.Network.NetworkManager.INetworkManagerCommand;
using JokerElectionUIController = Controllers.EveryBodyVsTheTeacher.Jokers.Election.JokerElectionUIController;

namespace Assets.Scripts.Commands.Jokers.Election
{

    using System;
    using System.Collections.Generic;

    using Assets.Scripts.Interfaces;

    using UnityEngine;

    public class StartedVotingForJokerCommand : INetworkManagerCommand
    {
        private readonly IJokerElectionUIController jokerElectionUiController;
        private readonly GameObject jokerElectionUI;
        private readonly IJoker joker;

        public StartedVotingForJokerCommand(
            IJokerElectionUIController jokerElectionUiController, 
            GameObject jokerElectionUI,
            IJoker joker)
        {
            if (jokerElectionUiController == null)
            {
                throw new ArgumentNullException("jokerElectionUiController");
            }

            if (jokerElectionUI == null)
            {
                throw new ArgumentNullException("jokerElectionUI");
            }

            if (joker == null)
            {
                throw new ArgumentNullException("joker");
            }

            this.jokerElectionUiController = jokerElectionUiController;
            this.jokerElectionUI = jokerElectionUI;
            this.joker = joker;
        }

        public void Execute(Dictionary<string, string> commandsOptionsValues)
        {
            if (this.jokerElectionUI.activeSelf)
            {
                throw new InvalidOperationException("Voting is already activated");
            }

            this.jokerElectionUI.SetActive(true);
            this.jokerElectionUiController.SetJokerImage(this.joker.Image);
        }
    }

}
