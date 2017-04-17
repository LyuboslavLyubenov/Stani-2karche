namespace Network
{

    using System;

    using Assets.Scripts.Network;

    using Commands.Server;

    using EventArgs;

    using Interfaces;
    using Interfaces.Network.NetworkManager;

    public class MainPlayerData : IPlayerData
    {
        public event EventHandler<ClientConnectionIdEventArgs> OnConnected = delegate { };
        public event EventHandler<ClientConnectionIdEventArgs> OnDisconnected = delegate { };

        private IServerNetworkManager networkManager;

        private readonly INetworkManagerCommand mainPlayerConnecting;

        public JokersData JokersData
        {
            get;
            private set;
        }

        public JokersUsedNotifier JokersUsedNotifier
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
            this.JokersData = new JokersData();
            this.JokersUsedNotifier = new JokersUsedNotifier(this.networkManager, this.JokersData);

            networkManager.OnClientDisconnected += this.OnClientDisconnected;
            networkManager.CommandsManager.AddCommand(this.mainPlayerConnecting);

            this.IsConnected = false;
        }

        private void OnClientDisconnected(object sender, ClientConnectionIdEventArgs args)
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

            this.OnConnected(this, new ClientConnectionIdEventArgs(connectionId));
        }
    }

}