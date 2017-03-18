namespace Assets.Scripts.Network.EveryBodyVsTheTeacher
{
    using System;

    using Assets.Scripts.Commands;
    using Assets.Scripts.Commands.EveryBodyVsTheTeacher.PlayersConnectingStateDataSender;
    using Assets.Scripts.Controllers.EveryBodyVsTheTeacher.States.Server;
    using Assets.Scripts.EventArgs;
    using Assets.Scripts.Interfaces.Network;
    using Assets.Scripts.Interfaces.Network.EveryBodyVsTheTeacher;
    using Assets.Scripts.Interfaces.Network.EveryBodyVsTheTeacher.States;
    using Assets.Scripts.Interfaces.Network.NetworkManager;

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

            playersConnectingState.OnMainPlayerConnected += OnMainPlayerConnected;
            playersConnectingState.OnMainPlayerDisconnected += OnMainPlayerDisconnected;

            playersConnectingState.OnAudiencePlayerConnected += OnAudiencePlayerConnected;
            playersConnectingState.OnAudiencePlayerDisconnected += OnAudiencePlayerDisconnected;

            playersConnectingState.OnMainPlayerRequestedGameStart += OnMainPlayerRequestedGameStart;
            playersConnectingState.OnEveryBodyRequestedGameStart += OnEveryBodyRequestedGameStart;
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
