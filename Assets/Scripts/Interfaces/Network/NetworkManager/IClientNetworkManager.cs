namespace Assets.Scripts.Interfaces.Network.NetworkManager
{

    using System;

    using Assets.Scripts.Commands;
    using Assets.Scripts.EventArgs;

    using UnityEngine;
    using UnityEngine.Networking;

    public interface IClientNetworkManager
    {
        CommandsManager CommandsManager
        {
            get;
        }

        event EventHandler OnConnectedEvent;

        event EventHandler<DataSentEventArgs> OnReceivedDataEvent;

        event EventHandler OnDisconnectedEvent;

        bool IsConnected
        {
            get;
        }
        
        NetworkConnectionError ConnectToHost(string ip);

        NetworkError Disconnect();

        void SendServerMessage(string message);

        void SendServerCommand(NetworkCommandData command);
    }

}
