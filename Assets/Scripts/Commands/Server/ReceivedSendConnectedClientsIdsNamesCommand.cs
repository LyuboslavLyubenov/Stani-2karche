using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

namespace Assets.Scripts.Commands.Server
{

    using Assets.Scripts.Interfaces;
    using Assets.Scripts.Network;

    public class ReceivedSendConnectedClientsIdsNamesCommand : INetworkManagerCommand
    {
        ServerNetworkManager networkManager;

        Dictionary<int, string> connectedClientsIdsNames;

        public ReceivedSendConnectedClientsIdsNamesCommand(ServerNetworkManager networkManager, Dictionary<int, string> connectedClientsIdsNames)
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
            var onlineClientsData = new OnlineClientsData_Serializable() { OnlinePlayers = clientsData };
            var connectedClientsDataJSON = JsonUtility.ToJson(onlineClientsData);

            commandData.AddOption("ConnectedClientsDataJSON", connectedClientsDataJSON);
            this.networkManager.SendClientCommand(clientConnectionId, commandData);
        }
    }

}

