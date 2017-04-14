namespace Commands.Server
{

    using System;
    using System.Collections.Generic;
    using System.Linq;

    using DTOs;

    using Interfaces.Network.NetworkManager;

    using UnityEngine;

    public class ServerSendConnectedClientsIdsNamesCommand : INetworkManagerCommand
    {
        private IServerNetworkManager networkManager;

        private Dictionary<int, string> connectedClientsIdsNames;

        public ServerSendConnectedClientsIdsNamesCommand(IServerNetworkManager networkManager, Dictionary<int, string> connectedClientsIdsNames)
        {       
            if (networkManager == null)
            {
                throw new ArgumentNullException("networkManager");   
            }

            if (connectedClientsIdsNames == null)
            {
                throw new ArgumentNullException("connectedClientsIdsNames");
            }

            this.networkManager = networkManager;
            this.connectedClientsIdsNames = connectedClientsIdsNames;
        }

        public void Execute(Dictionary<string, string> commandsParamsValues)
        {
            var clientConnectionId = int.Parse(commandsParamsValues["ConnectionId"]);
            var commandData = new NetworkCommandData("ConnectedClientsIdsNames");
            var clientsData = this.connectedClientsIdsNames.Where(c => c.Key != clientConnectionId)
                .Select(a => new ClientData_Serializable() { ConnectionId = a.Key, Username = a.Value })
                .ToArray();
            var onlineClientsData = new OnlineClientsData_DTO() { OnlinePlayers = clientsData };
            var connectedClientsDataJSON = JsonUtility.ToJson(onlineClientsData);

            commandData.AddOption("ConnectedClientsDataJSON", connectedClientsDataJSON);
            this.networkManager.SendClientCommand(clientConnectionId, commandData);
        }
    }
}