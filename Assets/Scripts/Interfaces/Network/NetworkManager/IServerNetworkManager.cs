namespace Assets.Scripts.Interfaces
{

    using System;

    using Assets.Scripts.Commands;
    using Assets.Scripts.EventArgs;

    public interface IServerNetworkManager
    {
        event EventHandler<ClientConnectionDataEventArgs> OnClientConnected;
        event EventHandler<DataSentEventArgs> OnReceivedData;
        event EventHandler<ClientConnectionDataEventArgs> OnClientDisconnected;
        event EventHandler<ConnectedClientDataEventArgs> OnClientSetUsername;//TODO Connectedclientdataeventargs is BULLSHIT

        ICommandsManager CommandsManager
        {
            get;
        }

        bool IsRunning
        {
            get;
        }

        int MaxConnections
        {
            get;
            set;
        }

        int ConnectedClientsCount
        {
            get;
        }

        int[] ConnectedClientsConnectionId
        {
            get;
        }

        string[] GetAllClientsNames();

        string GetClientUsername(int connectionId);

        void StartServer();

        void StopServer();

        void SetClientUsername(int connectionId, string username);

        void SendClientCommand(int connectionId, NetworkCommandData command);

        void SendClientMessage(int connectionId, string message);

        void SendAllClientsCommand(NetworkCommandData command);

        void SendAllClientsCommand(NetworkCommandData command, int exceptConnectionId);

        void SendAllClientsMessage(string message);

        void KickPlayer(int connectionId, string message);

        void KickPlayer(int connectionId);

        void BanPlayer(int connectionId);

        bool IsConnected(int connectionId);
    }

}