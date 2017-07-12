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

    public class JokerJokerElectionUiCommandsBinder : IJokerElectionCommandsBinder, IDisposable
    {
        protected readonly IClientNetworkManager networkManager;
        protected readonly IJokerElectionUIController jokerElectionUIController;
        protected readonly GameObject jokerElectionUI;

        protected readonly GameObject successfullyActivatedJokerUI;
        protected readonly GameObject unsuccessfullyActivatedJokerUI;

        private readonly List<string> jokerNamesCurrentlyListening = new List<string>();
        
        public JokerJokerElectionUiCommandsBinder(
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

            var playerVotedForCommand = new PlayerVotedForCommand(this.jokerElectionUIController);
            var playerVotedAgainstCommand = new PlayerVotedAgainstCommand(this.jokerElectionUIController);
            
            this.networkManager.CommandsManager.AddCommand(playerVotedForCommand);
            this.networkManager.CommandsManager.AddCommand(playerVotedAgainstCommand);
        }

        private void RemoveCommandsBindedToJoker(string jokerName)
        {
            this.networkManager.CommandsManager.RemoveCommand("ElectionStartedFor" + jokerName + "Joker");
            this.networkManager.CommandsManager.RemoveCommand("ElectionJokerResultFor" + jokerName + "Joker");
        }
        
        public void Bind(IJoker joker)
        {
            var jokerName = joker.GetName();

            if (this.jokerNamesCurrentlyListening.Contains(jokerName))
            {
                throw new InvalidOperationException("Already listening for " + jokerName + " joker");
            }

            var startedElectionCommand = 
                new StartedVotingForJokerCommand(
                    this.jokerElectionUIController, 
                    this.jokerElectionUI, 
                    joker);
            var electionJokerResultCommand =
                new ElectionJokerResultCommand(
                    this.jokerElectionUI,
                    this.successfullyActivatedJokerUI,
                    this.unsuccessfullyActivatedJokerUI);

            var commandsManager = this.networkManager.CommandsManager;
            commandsManager.AddCommand("ElectionStartedFor" + jokerName + "Joker", startedElectionCommand);
            commandsManager.AddCommand("ElectionJokerResultFor" + jokerName + "Joker", electionJokerResultCommand);

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

            this.networkManager.CommandsManager.RemoveCommand<PlayerVotedForCommand>();
            this.networkManager.CommandsManager.RemoveCommand<PlayerVotedAgainstCommand>();
            this.networkManager.CommandsManager.RemoveCommand<ElectionJokerResultCommand>();
        }
    }
}