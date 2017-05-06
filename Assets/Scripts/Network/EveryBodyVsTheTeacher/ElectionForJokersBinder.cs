using IClientNetworkManager = Interfaces.Network.NetworkManager.IClientNetworkManager;
using IJokerElectionUIController = Controllers.EveryBodyVsTheTeacher.Jokers.Election.IJokerElectionUIController;

namespace Assets.Scripts.Network.EveryBodyVsTheTeacher
{
    using System;
    using System.Collections.Generic;
    using Assets.Scripts.Commands.Jokers.Election;
    using Assets.Scripts.Extensions;
    using Assets.Scripts.Interfaces;
    using Assets.Scripts.Interfaces.Network.EveryBodyVsTheTeacher;

    using UnityEngine;

    public class ElectionForJokersBinder : IElectionForJokersBinder, IDisposable
    {
        private readonly IClientNetworkManager networkManager;
        private readonly IJokerElectionUIController jokerElectionUiController;
        private readonly GameObject jokerElectionUi;

        private readonly GameObject successfullyActivatedJokerUi;
        private readonly GameObject unsuccessfullyActivatedJokerUi;

        private readonly List<string> jokerNamesCurrentlyListening = new List<string>();
        
        public ElectionForJokersBinder(
            IClientNetworkManager networkManager,
            IJokerElectionUIController jokerElectionUIController, 
            GameObject jokerElectionUI,
            GameObject successfullyActivatedJokerUI,
            GameObject unsuccessfullyActivatedJokerUI)
        {
            if (networkManager == null)
            {
                throw new ArgumentNullException("networkManager");
            }

            if (jokerElectionUIController == null)
            {
                throw new ArgumentNullException("jokerElectionUIController");
            }

            if (jokerElectionUI == null)
            {
                throw new ArgumentNullException("jokerElectionUI");
            }

            if (successfullyActivatedJokerUI == null)
            {
                throw new ArgumentNullException("successfullyActivatedJokerUI");
            }

            if (unsuccessfullyActivatedJokerUI == null)
            {
                throw new ArgumentNullException("unsuccessfullyActivatedJokerUI");
            }

            this.networkManager = networkManager;
            this.jokerElectionUiController = jokerElectionUIController;
            this.jokerElectionUi = jokerElectionUI;
            this.successfullyActivatedJokerUi = successfullyActivatedJokerUI;
            this.unsuccessfullyActivatedJokerUi = unsuccessfullyActivatedJokerUI;
        }

        private void RemoveCommandsBindedToJoker(string jokerName)
        {
            this.networkManager.CommandsManager.RemoveCommand("ElectionStartedFor" + jokerName);
            this.networkManager.CommandsManager.RemoveCommand("PlayerVotedFor" + jokerName);
            this.networkManager.CommandsManager.RemoveCommand("PlayerVotedAgainst" + jokerName);
            this.networkManager.CommandsManager.RemoveCommand("ElectionResultFor" + jokerName);
        }
        
        public void Bind(IJoker joker)
        {
            var jokerName = joker.GetName();

            if (this.jokerNamesCurrentlyListening.Contains(jokerName))
            {
                throw new InvalidOperationException("Already listening for " + jokerName + " joker");
            }

            var startedElectionCommand = new StartedVotingForJokerCommand(this.jokerElectionUiController, this.jokerElectionUi, joker);
            var playerVotedForCommand = new VotedForJokerCommand(this.jokerElectionUiController);
            var playerVotedAgainstCommand = new VotedAgainstCommand(this.jokerElectionUiController);
            var electionResultCommand = new ElectionResultCommand(this.successfullyActivatedJokerUi, this.unsuccessfullyActivatedJokerUi);

            this.networkManager.CommandsManager.AddCommand("ElectionStartedFor" + jokerName, startedElectionCommand);
            this.networkManager.CommandsManager.AddCommand("PlayerVotedFor" + jokerName, playerVotedForCommand);
            this.networkManager.CommandsManager.AddCommand("PlayerVotedAgainst" + jokerName, playerVotedAgainstCommand);
            this.networkManager.CommandsManager.AddCommand("ElectionResultFor" + jokerName, electionResultCommand);

            this.jokerNamesCurrentlyListening.Add(jokerName);
        }

        public void Unbind(IJoker joker)
        {
            var jokerName = joker.GetName();

            if (this.jokerNamesCurrentlyListening.Contains(jokerName))
            {
                throw new InvalidOperationException("Already listening for " + jokerName + " joker");
            }
            
            this.RemoveCommandsBindedToJoker(jokerName);
            this.jokerNamesCurrentlyListening.Remove(jokerName);
        }

        public void Dispose()
        {
            for (int i = 0; i < this.jokerNamesCurrentlyListening.Count; i++)
            {
                var jokerName = this.jokerNamesCurrentlyListening[i];
                this.RemoveCommandsBindedToJoker(jokerName);
            }
            
            this.jokerNamesCurrentlyListening.Clear();
        }
    }
}
