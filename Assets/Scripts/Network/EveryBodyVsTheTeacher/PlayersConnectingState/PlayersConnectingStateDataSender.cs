namespace Network.EveryBodyVsTheTeacher
{

    using System;

    using Commands;
    using Commands.EveryBodyVsTheTeacher.PlayersConnectingStateDataSender;

    using EventArgs;

    using Interfaces.Network;
    using Interfaces.Network.EveryBodyVsTheTeacher;
    using Interfaces.Network.EveryBodyVsTheTeacher.States;
    using Interfaces.Network.NetworkManager;

    public class PlayersConnectingStateDataSender : IPlayersConnectingStateDataSender
    {
        private readonly IServerNetworkManager networkManager;

        private readonly IEveryBodyVsTheTeacherServer server;

        public PlayersConnectingStateDataSender(
            IPlayersConnectingToTheServerState playersConnectingState, 
            IServerNetworkManager networkManager,
            IEveryBodyVsTheTeacherServer server)
        {
            if (playersConnectingState == null)
            {
                throw new ArgumentNullException("playersConnectingState");
            }

            if (networkManager == null)
            {
                throw new ArgumentNullException("networkManager");
            }

            if (server == null)
            {
                throw new ArgumentNullException("server");
            }

            this.networkManager = networkManager;
            this.server = server;

            playersConnectingState.OnMainPlayerConnected += this.OnMainPlayerConnected;
            playersConnectingState.OnMainPlayerDisconnected += this.OnMainPlayerDisconnected;

            playersConnectingState.OnAudiencePlayerConnected += this.OnAudiencePlayerConnected;
            playersConnectingState.OnAudiencePlayerDisconnected += this.OnAudiencePlayerDisconnected;

            playersConnectingState.OnMainPlayerRequestedGameStart += this.OnMainPlayerRequestedGameStart;
            playersConnectingState.OnEveryBodyRequestedGameStart += this.OnEveryBodyRequestedGameStart;
        }

        private void OnEveryBodyRequestedGameStart(object sender, EventArgs args)
        {
            var everyBodyRequestedGameStart = NetworkCommandData.From<EveryBodyRequestedGameStartCommand>();
            this.networkManager.SendClientCommand(this.server.PresenterId, everyBodyRequestedGameStart);
        }

        private void OnMainPlayerRequestedGameStart(object sender, ClientConnectionDataEventArgs args)
        {
            var mainPlayerRequestedCommand = NetworkCommandData.From<MainPlayerRequestedGameStartCommand>();
            mainPlayerRequestedCommand.AddOption("ConnectionId", args.ConnectionId.ToString());
            this.networkManager.SendClientCommand(this.server.PresenterId, mainPlayerRequestedCommand);
        }

        private void OnAudiencePlayerDisconnected(object sender, ClientConnectionDataEventArgs args)
        {
            this.SendToPresenterClientDisconnected(args.ConnectionId, false);
        }

        private void OnAudiencePlayerConnected(object sender, ClientConnectionDataEventArgs args)
        {
            this.SendToPresenterClientConnected(args.ConnectionId, false);
        }

        private void OnMainPlayerDisconnected(object sender, ClientConnectionDataEventArgs args)
        {
            this.SendToPresenterClientDisconnected(args.ConnectionId, true);
        }

        private void OnMainPlayerConnected(object sender, ClientConnectionDataEventArgs args)
        {
            this.SendToPresenterClientConnected(args.ConnectionId, true);
        }

        private void SendToPresenterClientDisconnected(int connectionId, bool isMainPlayer)
        {
            var playerDisconnectedCommand = isMainPlayer
                                                ? NetworkCommandData.From<MainPlayerDisconnectedCommand>()
                                                : NetworkCommandData.From<AudiencePlayerDisconnectedCommand>();

            playerDisconnectedCommand.AddOption("ConnectionId", connectionId.ToString());
            this.networkManager.SendClientCommand(this.server.PresenterId, playerDisconnectedCommand);
        }

        private void SendToPresenterClientConnected(int connectionId, bool isMainPlayer)
        {
            var username = this.networkManager.GetClientUsername(connectionId);
            var playerConnectedCommand = 
                isMainPlayer 
                ? 
                NetworkCommandData.From<MainPlayerConnectedCommand>() 
                :
                NetworkCommandData.From<AudiencePlayerConnectedCommand>();

            playerConnectedCommand.AddOption("ConnectionId", connectionId.ToString());
            playerConnectedCommand.AddOption("Username", username);
            this.networkManager.SendClientCommand(this.server.PresenterId, playerConnectedCommand);
        }
    }
}
