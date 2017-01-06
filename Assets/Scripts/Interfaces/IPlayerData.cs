using System;

namespace Assets.Scripts.Interfaces
{

    using Assets.Scripts.EventArgs;

    public interface IPlayerData
    {
        event EventHandler<ClientConnectionDataEventArgs> OnConnected;

        event EventHandler<ClientConnectionDataEventArgs> OnDisconnected;

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