using IEveryBodyVsTheTeacherServer = Interfaces.Network.IEveryBodyVsTheTeacherServer;
using IServerNetworkManager = Interfaces.Network.NetworkManager.IServerNetworkManager;
using NetworkCommandData = Commands.NetworkCommandData;

namespace Assets.Scripts.Network.EveryBodyVsTheTeacher
{

    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Assets.Scripts.Interfaces.Commands.Jokers.Selected;

    using EventArgs;

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
                electionJokerCommand.OnAllPlayersSelected += this.OnAllPlayersSelectedJoker;
                electionJokerCommand.OnPlayerSelected += this.OnPlayerSelectedJoker;
                electionJokerCommand.OnSelectTimeout += this.OnSelectJokerTimeout;
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

        private void OnAllPlayersSelectedJoker(object sender, System.EventArgs args)
        {
            var jokerName = sender.GetType()
                .Name.Replace("Command", "")
                .Replace("Selected", "");
            var allPlayersSelectedCommand = new NetworkCommandData("AllPlayersSelected" + jokerName);
            this.SendToMainPlayersAndPresenter(allPlayersSelectedCommand);
        }

        private void OnSelectJokerTimeout(object sender, EventArgs args)
        {
            var jokerName = sender.GetType()
                .Name.Replace("Command", "")
                .Replace("Selected", "");
            var commandName = string.Format("Select{0}Timeout", jokerName);
            var selectTimeoutCommand = new NetworkCommandData(commandName);
            this.SendToMainPlayersAndPresenter(selectTimeoutCommand);
        }

        private void OnPlayerSelectedJoker(object sender, ClientConnectionIdEventArgs args)
        {
            var jokerName = sender.GetType()
                .Name.Replace("Command", "")
                .Replace("Selected", "");
            var playerSelectedCommand = new NetworkCommandData("OnPlayerSelected" + jokerName);
            playerSelectedCommand.AddOption("ConnectionId", args.ConnectionId.ToString());
            networkManager.SendClientCommand(server.PresenterId, playerSelectedCommand);
        }
    }
}
