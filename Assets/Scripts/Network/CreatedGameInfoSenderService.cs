namespace Assets.Scripts.Network
{
    using System;

    using Interfaces;

    using NetworkManagers;

    using UnityEngine;
    using EventArgs;

    using TcpSockets;

    public class CreatedGameInfoSenderService : IDisposable
    {
        public const string GameInfoTag = "[CreatedGameInfo]";
        public const string SendGameInfoCommandTag = "[SendGameInfo]";

        private readonly SimpleTcpClient client;
        private readonly SimpleTcpServer server;

        private GameInfoFactory gameInfoFactory;

        private readonly ServerNetworkManager serverNetworkManager;

        private readonly IGameServer gameServer;

        public CreatedGameInfoSenderService(SimpleTcpClient client,
                                            SimpleTcpServer server,
                                            GameInfoFactory gameInfoFactory,
                                            ServerNetworkManager serverNetworkManager,
                                            IGameServer gameServer)
        {
            if (client == null)
            {
                throw new ArgumentNullException("client");
            }

            if (server == null)
            {
                throw new ArgumentNullException("server");
            }

            if (gameInfoFactory == null)
            {
                throw new ArgumentNullException("gameInfoFactory");
            }

            if (serverNetworkManager == null)
            {
                throw new ArgumentNullException("serverNetworkManager");
            }

            if (gameServer == null)
            {
                throw new ArgumentNullException("gameServer");
            }

            this.client = client;
            this.server = server;
            this.gameInfoFactory = gameInfoFactory;
            this.serverNetworkManager = serverNetworkManager;
            this.gameServer = gameServer;

            this.server.OnReceivedMessage += this.OnReceivedMessage;
        }

        private void OnReceivedMessage(object sender, MessageEventArgs args)
        {
            if (!args.Message.Contains(SendGameInfoCommandTag))
            {
                return;
            }

            if (!this.client.IsConnectedTo(args.IPAddress))
            {
                this.client.ConnectTo(args.IPAddress, 7772, () => SendGameInfo(args.IPAddress));
                return;
            }

            SendGameInfo(args.IPAddress);
        }

        private void SendGameInfo(string ipAddress)
        {
            var gameInfo = this.gameInfoFactory.Get(this.serverNetworkManager, this.gameServer);
            var gameInfoJSON = JsonUtility.ToJson(gameInfo);
            var messageToSend = GameInfoTag + gameInfoJSON;

            this.client.Send(ipAddress, messageToSend,
                () =>
                    {
                        this.client.DisconnectFrom(ipAddress);
                    }, Debug.LogException);
        }

        public void Dispose()
        {
            this.client.Dispose();
            this.server.Dispose();
        }
    }
}