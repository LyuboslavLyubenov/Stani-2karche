using System;

namespace Assets.Scripts.Interfaces
{

    using Assets.Scripts.EventArgs;

    public interface IPlayerData
    {
        EventHandler<ClientConnectionDataEventArgs> OnConnected
        {
            get;
            set;
        }

        EventHandler<ClientConnectionDataEventArgs> OnDisconnected
        {
            get;
            set;
        }

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