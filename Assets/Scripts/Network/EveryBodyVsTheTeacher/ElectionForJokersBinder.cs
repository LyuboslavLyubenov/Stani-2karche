using IClientNetworkManager = Interfaces.Network.NetworkManager.IClientNetworkManager;
using IJokerElectionUIController = Assets.Scripts.Interfaces.Controllers.IJokerElectionUIController;

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
        private readonly IJokerElectionUIController jokerElectionUIController;
        private readonly GameObject jokerElectionUI;

        private readonly GameObject successfullyActivatedJokerUI;
        private readonly GameObject unsuccessfullyActivatedJokerUI;

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
            this.jokerElectionUIController = jokerElectionUIController;
            this.jokerElectionUI = jokerElectionUI;
            this.successfullyActivatedJokerUI = successfullyActivatedJokerUI;
            this.unsuccessfullyActivatedJokerUI = unsuccessfullyActivatedJokerUI;
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

            var startedElectionCommand = new StartedVotingForJokerCommand(this.jokerElectionUIController, this.jokerElectionUI, joker);
            var playerVotedForCommand = new VotedForJokerCommand(this.jokerElectionUIController);
            var playerVotedAgainstCommand = new VotedAgainstCommand(this.jokerElectionUIController);
            var electionResultCommand = new ElectionResultCommand(this.successfullyActivatedJokerUI, this.unsuccessfullyActivatedJokerUI);

            this.networkManager.CommandsManager.AddCommand("ElectionStartedFor" + jokerName + "Joker", startedElectionCommand);
            this.networkManager.CommandsManager.AddCommand("PlayerVotedFor" + jokerName + "Joker", playerVotedForCommand);
            this.networkManager.CommandsManager.AddCommand("PlayerVotedAgainst" + jokerName + "Joker", playerVotedAgainstCommand);
            this.networkManager.CommandsManager.AddCommand("ElectionResultFor" + jokerName + "Joker", electionResultCommand);

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
