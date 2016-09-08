using System;
using System.Collections.Generic;

public class ServerSendConnectedClientsCountCommand : INetworkManagerCommand
{
    ServerNetworkManager networkManager;

    public ServerSendConnectedClientsCountCommand(ServerNetworkManager networkManager)
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
        commandData.AddOption("ConnectedClientsCount", networkManager.ConnectedClientsCount.ToString());
        networkManager.SendClientCommand(connectionId, commandData);
    }

}
