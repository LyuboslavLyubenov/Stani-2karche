namespace Network.GameInfo
{

    using System;
    using System.Collections.Generic;

    using EventArgs;

    using Interfaces.Network;
    using Interfaces.Network.NetworkManager;

    public class CreatedGameInfoReceiver : ICreatedGameInfoReceiver
    {
        private readonly Dictionary<string, Action<GameInfoEventArgs>> pendingRequests = new Dictionary<string, Action<GameInfoEventArgs>>();

        private ISimpleTcpClient client;
        private ISimpleTcpServer server;
        
        public CreatedGameInfoReceiver(ISimpleTcpClient client, ISimpleTcpServer server)
        {
            if (client == null)
            {
                throw new ArgumentNullException("client");
            }

            if (server == null)
            {
                throw new ArgumentNullException("server");
            }
            
            this.client = client;
            this.server = server;

            this.server.OnReceivedMessage += this.OnReceivedMessage;
        }

        private void OnReceivedMessage(object sender, MessageEventArgs args)
        {
            var gameInfoTagIndex = args.Message.IndexOf(CreatedGameInfoSender.GameInfoTag, StringComparison.Ordinal);

            if (!this.pendingRequests.ContainsKey(args.IPAddress) || gameInfoTagIndex < 0)
            {
                return;
            }

            UnityEngine.Debug.Log("Received gameinfo from " + args.IPAddress);

            var filteredMessage = args.Message.Remove(gameInfoTagIndex, CreatedGameInfoSender.GameInfoTag.Length);
            var gameInfo = new GameInfoEventArgs(filteredMessage);

            var requestCallback = this.pendingRequests[args.IPAddress];

            this.pendingRequests.Remove(args.IPAddress);

            requestCallback(gameInfo);
        }

        private void _ReceiveFrom(string ipAddress, Action<GameInfoEventArgs> receivedGameInfo, Action<Exception> onError = null)
        {
            this.client.Send(ipAddress, CreatedGameInfoSender.SendGameInfoCommandTag, null,
                (exception) =>
                    {
                        this.pendingRequests.Remove(ipAddress);

                        if (onError != null)
                        {
                            onError(exception);
                        }
                    });
            this.pendingRequests.Add(ipAddress, receivedGameInfo);
        }

        public void ReceiveFrom(string ipAddress, Action<GameInfoEventArgs> receivedGameInfo, Action<Exception> onError = null)
        {
            UnityEngine.Debug.Log("Start receiving game info from " + ipAddress);

            if (this.client.IsConnectedTo(ipAddress))
            {
                this._ReceiveFrom(ipAddress, receivedGameInfo, onError);
                return;
            }

            this.client.ConnectTo(ipAddress, this.server.Port,
                () =>
                {
                    this._ReceiveFrom(ipAddress, receivedGameInfo, onError);
                },
                (exception) =>
                    {
                        this.pendingRequests.Remove(ipAddress);

                        if (onError != null)
                        {
                            onError(exception);
                        }
                    });
        }

        public void StopReceivingFrom(string ipAddress)
        {
            if (!this.pendingRequests.ContainsKey(ipAddress))
            {
                throw new InvalidOperationException("Not listening to this ipAddress");
            }

            this.pendingRequests.Remove(ipAddress);
        }

        public void StopReceivingFromAll()
        {
            this.pendingRequests.Clear();
        }
    }
}