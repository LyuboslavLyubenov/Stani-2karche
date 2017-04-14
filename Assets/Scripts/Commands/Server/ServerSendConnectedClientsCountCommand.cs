namespace Commands.Server
{

    using System;
    using System.Collections.Generic;

    using Interfaces.Network.NetworkManager;

    public class ServerSendConnectedClientsCountCommand : INetworkManagerCommand
    {
        private IServerNetworkManager networkManager;

        public ServerSendConnectedClientsCountCommand(IServerNetworkManager networkManager)
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
