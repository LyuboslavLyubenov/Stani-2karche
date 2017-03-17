namespace Assets.Scripts.Network
{
    using System;
    using System.Collections.Generic;

    using Assets.Scripts.Interfaces;
    using Assets.Scripts.Interfaces.Network;
    using Assets.Scripts.Interfaces.Network.NetworkManager;

    using EventArgs;
    using TcpSockets;

    public class CreatedGameInfoReceiver : ICreatedGameInfoReceiver
    {
        private readonly Dictionary<string, Action<GameInfoReceivedDataEventArgs>> pendingRequests = new Dictionary<string, Action<GameInfoReceivedDataEventArgs>>();

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

            var filteredMessage = args.Message.Remove(gameInfoTagIndex, CreatedGameInfoSender.GameInfoTag.Length);
            var gameInfo = new GameInfoReceivedDataEventArgs(filteredMessage);

            var requestCallback = this.pendingRequests[args.IPAddress];

            this.pendingRequests.Remove(args.IPAddress);

            requestCallback(gameInfo);
        }

        private void _ReceiveFrom(string ipAddress, Action<GameInfoReceivedDataEventArgs> receivedGameInfo, Action<Exception> onError = null)
        {
            this.client.Send(ipAddress, CreatedGameInfoSender.SendGameInfoCommandTag, null, onError);
            this.pendingRequests.Add(ipAddress, receivedGameInfo);
        }

        public void ReceiveFrom(string ipAddress, Action<GameInfoReceivedDataEventArgs> receivedGameInfo, Action<Exception> onError = null)
        {
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