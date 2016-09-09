using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ServerSendConnectedClientsIdsNamesCommand : INetworkManagerCommand
{
    ServerNetworkManager networkManager;

    Dictionary<int, string> connectedClientsIdsNames;

    public ServerSendConnectedClientsIdsNamesCommand(ServerNetworkManager networkManager, Dictionary<int, string> connectedClientsIdsNames)
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
        var connectedClientsData = connectedClientsIdsNames.Select(a => new ConnectedClientData(a.Key, a.Value)).ToArray();
        var connectedClientsDataJSON = JsonUtility.ToJson(connectedClientsData);

        commandData.AddOption("ConnectedClientsDataJSON", connectedClientsDataJSON);
        networkManager.SendClientCommand(clientConnectionId, commandData);
    }
}

