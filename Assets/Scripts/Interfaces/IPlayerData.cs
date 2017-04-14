namespace Interfaces
{

    using System;

    using EventArgs;

    public interface IPlayerData
    {
        event EventHandler<ClientConnectionIdEventArgs> OnConnected;

        event EventHandler<ClientConnectionIdEventArgs> OnDisconnected;

        bool IsConnected
        {
            get;
        }

        int ConnectionId
        {
            get;
        }

        string Username
        {
            get;
        }
    }

    public interface IClientConnectionStatusTracker : IDisposable
    {
        event EventHandler<ClientConnectionIdEventArgs> OnConnected;

        event EventHandler<ClientConnectionIdEventArgs> OnDisconnected;

        bool IsConnected
        {
            get;
        }

        int ConnectionId
        {
            get;
        }

        string Username
        {
            get;
        }
    }

}