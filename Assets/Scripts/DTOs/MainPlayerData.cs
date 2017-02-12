namespace Assets.Scripts.DTOs
{
    using System;

    using Commands.Server;
    using EventArgs;
    using Interfaces;
    using Network.NetworkManagers;

    public class MainPlayerData : IPlayerData
    {

        public event EventHandler<ClientConnectionDataEventArgs> OnConnected = delegate { };

        public event EventHandler<ClientConnectionDataEventArgs> OnDisconnected = delegate { };

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

        private ServerNetworkManager networkManager;

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
            
            this.IsConnected = false;
        }

        private void OnClientDisconnected(object sender, ClientConnectionDataEventArgs args)
        {
            if (this.ConnectionId != args.ConnectionId)
            {
                return;
            }

            this.networkManager.CommandsManager.AddCommand(new MainPlayerConnectingCommand(this.OnMainPlayerConnecting));

            this.IsConnected = false;

            this.OnDisconnected(this, args);
        }

        private void OnMainPlayerConnecting(int connectionId)
        {
            this.ConnectionId = connectionId;
            this.IsConnected = true;

            this.networkManager.CommandsManager.RemoveCommand<MainPlayerConnectingCommand>();

            this.OnConnected(this, new ClientConnectionDataEventArgs(connectionId));
        }
    }

}