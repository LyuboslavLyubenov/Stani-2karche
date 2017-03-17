namespace Network
{

    using System;

    using Assets.Scripts.Commands.Server;
    using Assets.Scripts.DTOs;
    using Assets.Scripts.EventArgs;
    using Assets.Scripts.Interfaces;
    using Assets.Scripts.Interfaces.Network.NetworkManager;

    public class MainPlayerData : IPlayerData
    {
        public event EventHandler<ClientConnectionDataEventArgs> OnConnected = delegate { };
        public event EventHandler<ClientConnectionDataEventArgs> OnDisconnected = delegate { };

        private IServerNetworkManager networkManager;

        private readonly INetworkManagerCommand mainPlayerConnecting;

        public JokersData JokersData
        {
            get;
            private set;
        }

        public bool IsConnected
        {
            get;
            private set;
        }

        public int ConnectionId
        {
            get;
            private set;
        }

        public string Username
        {
            get
            {
                if (!this.IsConnected)
                {
                    throw new Exception("MainPlayer not connected to server");
                }

                return this.networkManager.GetClientUsername(this.ConnectionId);
            }
        }

        public MainPlayerData(IServerNetworkManager networkManager)
        {
            if (networkManager == null)
            {
                throw new ArgumentNullException("networkManager");
            }

            this.networkManager = networkManager;

            this.mainPlayerConnecting = new MainPlayerConnectingCommand(this.OnMainPlayerConnecting);
            this.JokersData = new JokersData(networkManager);

            networkManager.OnClientDisconnected += this.OnClientDisconnected;
            networkManager.CommandsManager.AddCommand(this.mainPlayerConnecting);

            this.IsConnected = false;
        }

        private void OnClientDisconnected(object sender, ClientConnectionDataEventArgs args)
        {
            if (this.ConnectionId != args.ConnectionId)
            {
                return;
            }

            this.networkManager.CommandsManager.AddCommand(this.mainPlayerConnecting);

            this.IsConnected = false;
            this.OnDisconnected(this, args);
        }

        private void OnMainPlayerConnecting(int connectionId)
        {
            this.ConnectionId = connectionId;
            this.IsConnected = true;

            this.OnConnected(this, new ClientConnectionDataEventArgs(connectionId));
        }
    }

}