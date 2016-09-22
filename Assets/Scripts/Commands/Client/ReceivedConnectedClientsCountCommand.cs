using UnityEngine;
using System.Collections;
using System;

public class ReceivedConnectedClientsCountCommand : INetworkManagerCommand
{
    ValueWrapper<int> serverConnectedClientsCount;

    public ReceivedConnectedClientsCountCommand(ValueWrapper<int> serverConnectedClientsCount)
    {
        if (serverConnectedClientsCount == null)
        {
            throw new ArgumentNullException("serverConnectedClientsCount");
        }
            
        this.serverConnectedClientsCount = serverConnectedClientsCount;       
    }

    public void Execute(System.Collections.Generic.Dictionary<string, string> commandsOptionsValues)
    {
        var clientsCount = int.Parse(commandsOptionsValues["ConnectedClientsCount"]);
        serverConnectedClientsCount.Value = clientsCount;
    }
}
