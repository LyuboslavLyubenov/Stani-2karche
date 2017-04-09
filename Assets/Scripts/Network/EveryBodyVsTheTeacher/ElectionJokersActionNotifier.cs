﻿using ClientConnectionIdEventArgs = EventArgs.ClientConnectionIdEventArgs;
using IEveryBodyVsTheTeacherServer = Interfaces.Network.IEveryBodyVsTheTeacherServer;
using IServerNetworkManager = Interfaces.Network.NetworkManager.IServerNetworkManager;
using NetworkCommandData = Commands.NetworkCommandData;

namespace Network.EveryBodyVsTheTeacher
{

    using System;
    using System.Collections.Generic;
    using System.Linq;

    using EventArgs.Jokers;

    using Interfaces.Commands.Jokers.Selected;

    public class ElectionJokersActionNotifier
    {
        private readonly IServerNetworkManager networkManager;
        private readonly IEveryBodyVsTheTeacherServer server;

        /// <summary>
        /// Notifies main players and presenter when mainplayers started vote for joker or finished voting 
        /// </summary>
        public ElectionJokersActionNotifier(
            IServerNetworkManager networkManager, 
            IEveryBodyVsTheTeacherServer server, 
            IElectionJokerCommand[] electionJokerCommands)
        {
            if (networkManager == null)
            {
                throw new ArgumentNullException("networkManager");
            }

            if (server == null)
            {
                throw new ArgumentNullException("server");
            }

            if (electionJokerCommands == null || 
                electionJokerCommands.Length == 0 ||
                !electionJokerCommands.Select(c => c != null).Any())
            {
                throw new ArgumentNullException("electionJokerCommands");
            }
            
            this.networkManager = networkManager;
            this.server = server;

            for (int i = 0; i < electionJokerCommands.Length; i++)
            {
                var electionJokerCommand = electionJokerCommands[i];
                electionJokerCommand.OnElectionResult += this.OnElectionResultJoker;
                electionJokerCommand.OnPlayerSelectedFor += this.OnPlayerSelectedFor;
                electionJokerCommand.OnPlayerSelectedAgainst += this.OnPlayerSelectedAgainst;
            }
        }
        
        private void SendToMainPlayersAndPresenter(NetworkCommandData command)
        {
            var clientsToSendTo = new List<int>();

            clientsToSendTo.AddRange(this.server.MainPlayersConnectionIds);
            clientsToSendTo.Add(this.server.PresenterId);

            for (int i = 0; i < clientsToSendTo.Count; i++)
            {
                var connectionId = clientsToSendTo[i];
                this.networkManager.SendClientCommand(connectionId, command);
            }
        }

        private string GetJokerName(object jokerCommand)
        {
            return jokerCommand.GetType()
                .Name.Replace("Command", "")
                .Replace("Selected", "");
        }
        
        private void OnElectionResultJoker(object sender, ElectionJokerResultEventArgs args)
        {
            var jokerName = this.GetJokerName(sender);
            var command = new NetworkCommandData("ElectionResultFor" + jokerName);
            command.AddOption("Decision", args.ElectionDecision.ToString());
            this.SendToMainPlayersAndPresenter(command);
        }
        
        private void OnPlayerSelectedFor(object sender, ClientConnectionIdEventArgs args)
        {
            var jokerName = this.GetJokerName(sender);
            var command = new NetworkCommandData("PlayerSelectedFor" + jokerName);
            this.networkManager.SendClientCommand(this.server.PresenterId, command);
        }
        
        private void OnPlayerSelectedAgainst(object sender, ClientConnectionIdEventArgs args)
        {
            var jokerName = this.GetJokerName(sender);
            var command = new NetworkCommandData("PlayerSelectedAgainst" + jokerName);
            this.networkManager.SendClientCommand(this.server.PresenterId, command);
        }
    }
}
