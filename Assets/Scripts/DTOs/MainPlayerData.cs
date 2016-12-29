namespace Assets.Scripts.DTOs
{

    using System;

    using Assets.Scripts.Commands.Server;
    using Assets.Scripts.EventArgs;
    using Assets.Scripts.Interfaces;
    using Assets.Scripts.Network;
    using Assets.Scripts.Network.NetworkManagers;

    public class MainPlayerData : IPlayerData
    {
        ServerNetworkManager networkManager;

        public EventHandler<ClientConnectionDataEventArgs> OnConnected
        {
            get;
            set;
        }

        public EventHandler<ClientConnectionDataEventArgs> OnDisconnected
        {
            get;
            set;
        }

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

        public MainPlayerData(ServerNetworkManager networkManager)
        {
            if (networkManager == null)
            {
                throw new ArgumentNullException("networkManager");
            }

            this.networkManager = networkManager;
            this.JokersData = new JokersData(networkManager);

            networkManager.OnClientDisconnected += this.OnClientDisconnected;
            networkManager.CommandsManager.AddCommand(new MainPlayerConnectingCommand(this.OnMainPlayerConnecting));

            //lazy motherf*cker
            this.OnConnected = delegate
                {
                };

            this.OnDisconnected = delegate
                {
                };

            this.IsConnected = false;
        }

        void OnClientDisconnected(object sender, ClientConnectionDataEventArgs args)
        {
            if (this.ConnectionId != args.ConnectionId)
            {
                return;
            }

            this.networkManager.CommandsManager.AddCommand(new MainPlayerConnectingCommand(this.OnMainPlayerConnecting));

            this.IsConnected = false;
            this.OnDisconnected(this, args);
        }

        void OnMainPlayerConnecting(int connectionId)
        {
            this.ConnectionId = connectionId;
            this.IsConnected = true;

            this.networkManager.CommandsManager.RemoveCommand<MainPlayerConnectingCommand>();

            this.OnConnected(this, new ClientConnectionDataEventArgs(connectionId));
        }
    }

}