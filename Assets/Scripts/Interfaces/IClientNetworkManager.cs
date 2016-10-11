using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public interface IClientNetworkManager//TODO: napishi prehodnik za Clientnetowkrmanager kogato client-a e client i server
{
    EventHandler OnConnectedEvent
    {
        get;
        set;
    }

    EventHandler<DataSentEventArgs> OnReceivedDataEvent
    {
        get;
        set;
    }

    EventHandler OnDisconnectedEvent
    {
        get;
        set;
    }

    bool IsConnected
    {
        get;
    }

    int ServerConnectedClientsCount
    {
        get;
    }

    void ConnectToHost(string ip);

    void Disconnect();

    void SendServerMessage(string message);

    void SendServerCommand(NetworkCommandData command);
}
