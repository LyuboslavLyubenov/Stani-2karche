namespace Assets.Scripts.Network
{
    using System;
    using System.Collections.Generic;

    using EventArgs;
    using TcpSockets;

    public class CreatedGameInfoReceiverService
    {
        private readonly Dictionary<string, Action<GameInfoReceivedDataEventArgs>> pendingRequests = new Dictionary<string, Action<GameInfoReceivedDataEventArgs>>();

        private SimpleTcpClient client;
        private SimpleTcpServer server;

        public CreatedGameInfoReceiverService(SimpleTcpClient client, SimpleTcpServer server)
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
            var gameInfoTagIndex = args.Message.IndexOf(CreatedGameInfoSenderService.GameInfoTag);

            if (!this.pendingRequests.ContainsKey(args.IPAddress) || gameInfoTagIndex < 0)
            {
                return;
            }

            var filteredMessage = args.Message.Remove(gameInfoTagIndex, CreatedGameInfoSenderService.GameInfoTag.Length);
            var gameInfo = new GameInfoReceivedDataEventArgs(filteredMessage);

            this.pendingRequests[args.IPAddress](gameInfo);
            this.pendingRequests.Remove(args.IPAddress);
        }

        public void ReceiveFrom(string ipAddress, Action<GameInfoReceivedDataEventArgs> receivedGameInfo, Action<Exception> onError = null)
        {
            this.client.ConnectTo(ipAddress, this.server.Port, () =>
                {
                    this.client.Send(ipAddress, CreatedGameInfoSenderService.SendGameInfoCommandTag, null, onError);
                    this.pendingRequests.Add(ipAddress, receivedGameInfo);
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
    }

}
