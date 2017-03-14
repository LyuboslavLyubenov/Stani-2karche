namespace Assets.Tests.DummyObjects
{

    using System;

    using Assets.Scripts.Commands;
    using Assets.Scripts.EventArgs;
    using Assets.Scripts.Interfaces.Network.NetworkManager;

    using UnityEngine;
    using UnityEngine.Networking;

    public class DummyClientNetworkManager : IClientNetworkManager
    {
        public event EventHandler OnConnectedEvent = delegate { };

        public event EventHandler<DataSentEventArgs> OnReceivedDataEvent = delegate { };

        public event EventHandler OnDisconnectedEvent = delegate { };

        public event EventHandler<DataSentEventArgs> OnSentToServerMessage = delegate
            { };

        public CommandsManager CommandsManager
        {
            get; private set;
        }

        public bool IsConnected
        {
            get; private set;
        }
        
        public DummyClientNetworkManager()
        {
            this.CommandsManager = new CommandsManager();
        }

        public NetworkConnectionError ConnectToHost(string ip)
        {
            if (this.IsConnected)
            {
                return NetworkConnectionError.AlreadyConnectedToServer;
            }

            this.IsConnected = true;
            this.OnConnectedEvent(this, EventArgs.Empty);
            return NetworkConnectionError.NoError;
        }

        public NetworkError Disconnect()
        {
            this.IsConnected = false;
            this.OnDisconnectedEvent(this, EventArgs.Empty);
            return NetworkError.Ok;
        }

        public void SendServerMessage(string message)
        {
            this.OnSentToServerMessage(this, new DataSentEventArgs(0, message));
        }

        public void SendServerCommand(NetworkCommandData command)
        {
            this.SendServerMessage(command.ToString());
        }

        public void FakeReceiveMessage(string message)
        {
            this.OnReceivedDataEvent(this, new DataSentEventArgs(0, message));
        }
    }
}
