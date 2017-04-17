namespace Network
{

    using System;

    using EventArgs;

    using Interfaces;
    using Interfaces.Network.NetworkManager;

    public class ClientConnectionStatusTracker : IClientConnectionStatusTracker
    {
        public event EventHandler<ClientConnectionIdEventArgs> OnConnected = delegate { };
        public event EventHandler<ClientConnectionIdEventArgs> OnDisconnected = delegate { };

        private IServerNetworkManager networkManager;

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
                return !this.IsConnected ? "" : this.networkManager.GetClientUsername(this.ConnectionId);
            }
        }

        public ClientConnectionStatusTracker(IServerNetworkManager networkManager, int connectionId)
        {
            if (networkManager == null)
            {
                throw new ArgumentNullException("networkManager");
            }

            if (connectionId <= 0)
            {
                throw new ArgumentOutOfRangeException("connectionId");
            }

            this.ConnectionId = connectionId;
            this.networkManager = networkManager;
            
            this.networkManager.OnClientConnected += this.OnClientConnected;
            this.networkManager.OnClientDisconnected += this.OnClientDisconnected;

            this.IsConnected = false;
        }

        private void OnClientConnected(object sender, ClientConnectionIdEventArgs args)
        {
            if (this.ConnectionId != args.ConnectionId)
            {
                return;
            }

            this.IsConnected = true;
            this.OnConnected(this, args);
        }

        private void OnClientDisconnected(object sender, ClientConnectionIdEventArgs args)
        {
            if (this.ConnectionId != args.ConnectionId)
            {
                return;
            }
            
            this.IsConnected = false;
            this.OnDisconnected(this, args);
        }

        public void Dispose()
        {
            this.networkManager.OnClientConnected -= this.OnClientConnected;
            this.networkManager.OnClientDisconnected -= this.OnClientDisconnected;
        }
    }

}