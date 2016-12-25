using System;
using System.Collections.Generic;

namespace Assets.Scripts.Commands.Server
{

    using Assets.Scripts.Interfaces;
    using Assets.Scripts.Network;

    public class ReceivedSendConnectedClientsCountCommand : INetworkManagerCommand
    {
        ServerNetworkManager networkManager;

        public ReceivedSendConnectedClientsCountCommand(ServerNetworkManager networkManager)
        {
            if (networkManager == null)
            {
                throw new ArgumentNullException("networkManager");
            }
            
            this.networkManager = networkManager;
        }

        public void Execute(Dictionary<string, string> commandsOptionsValues)
        {
            var connectionId = int.Parse(commandsOptionsValues["ConnectionId"]);
            var commandData = new NetworkCommandData("ConnectedClientsCount");
            commandData.AddOption("ConnectedClientsCount", this.networkManager.ConnectedClientsCount.ToString());
            this.networkManager.SendClientCommand(connectionId, commandData);
        }

    }

}
