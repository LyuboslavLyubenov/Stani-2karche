using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class ClientReceiveConnectedClientsDataCommand : IOneTimeExecuteCommand
{
    Action<OnlineClientsData_Serializable> onReceivedOnlineClients;

    public bool FinishedExecution
    {
        get;
        private set;
    }

    public ClientReceiveConnectedClientsDataCommand(Action<OnlineClientsData_Serializable> onReceivedOnlineClientsData)
    {
        if (onReceivedOnlineClientsData == null)
        {
            throw new ArgumentNullException("onReceivedOnlineClientsData");
        }
            
        this.onReceivedOnlineClients = onReceivedOnlineClientsData;
        FinishedExecution = false;
    }

    public void Execute(Dictionary<string, string> commandsOptionsValues)
    {
        var connectedClientsDataJSON = commandsOptionsValues["ConnectedClientsDataJSON"];
        var onlineClientsData = JsonUtility.FromJson<OnlineClientsData_Serializable>(connectedClientsDataJSON);
        onReceivedOnlineClients(onlineClientsData);
        FinishedExecution = true;
    }
}
