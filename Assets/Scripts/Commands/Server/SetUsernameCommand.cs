using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class SetUsernameCommand : INetworkManagerCommand
{
    ServerNetworkManager networkManager;

    public SetUsernameCommand(ServerNetworkManager networkManager)
    {
        if (networkManager == null)
        {
            throw new ArgumentNullException("networkManager");
        }

        this.networkManager = networkManager;
    }

    public void Execute(Dictionary<string, string> commandsParamsValues)
    {
        var connectionId = int.Parse(commandsParamsValues["ConnectionId"]);

        if (!commandsParamsValues.ContainsKey("Username"))
        {
            //empty username :(
            networkManager.SetClientName(connectionId, "Играч " + connectionId);
            return;
        }
        else
        {
            var username = commandsParamsValues["Username"];
            networkManager.SetClientName(connectionId, username);   
        }
    }
    
}
