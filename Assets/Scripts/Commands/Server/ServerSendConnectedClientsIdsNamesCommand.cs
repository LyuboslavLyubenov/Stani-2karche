using System;
using System.Collections.Generic;
using UnityEngine;

public class ServerSendConnectedClientsIdsNamesCommand : INetworkManagerCommand
{
    ServerNetworkManager networkManager;

    OnlineClientsData_Serializable connectedClients;

    public ServerSendConnectedClientsIdsNamesCommand(ServerNetworkManager networkManager, OnlineClientsData_Serializable connectedClients)
    {
        if (connectedClients == null)
        {
            throw new ArgumentNullException("connectedClients");
        }
            
        if (networkManager == null)
        {
            throw new System.ArgumentNullException("networkManager");   
        }

        this.networkManager = networkManager;
        this.connectedClients = connectedClients;
    }

    public void Execute(Dictionary<string, string> commandsParamsValues)
    {
        var clientConnectionId = int.Parse(commandsParamsValues["ConnectionId"]);
        var commandData = new NetworkCommandData("ConnectedClientsIdsNames");
        var connectedClientsDataJSON = JsonUtility.ToJson(connectedClients);

        commandData.AddOption("ConnectedClientsDataJSON", connectedClientsDataJSON);
        networkManager.SendClientCommand(clientConnectionId, commandData);
    }
}

