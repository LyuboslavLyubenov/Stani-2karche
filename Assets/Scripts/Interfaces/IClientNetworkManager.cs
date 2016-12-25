using System;

namespace Assets.Scripts.Interfaces
{

    using Assets.Scripts.Commands;
    using Assets.Scripts.EventArgs;

    public interface IClientNetworkManager
    {
        CommandsManager CommandsManager
        {
            get;
        }

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

}
