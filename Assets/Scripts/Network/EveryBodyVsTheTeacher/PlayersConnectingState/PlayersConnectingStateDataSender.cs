namespace Network.EveryBodyVsTheTeacher.PlayersConnectingState
{
    using System;
    using System.Linq;

    using Commands;
    using Commands.EveryBodyVsTheTeacher;
    using Commands.EveryBodyVsTheTeacher.PlayersConnectingState;

    using EventArgs;

    using Interfaces.Network;
    using Interfaces.Network.EveryBodyVsTheTeacher;
    using Interfaces.Network.EveryBodyVsTheTeacher.States;
    using Interfaces.Network.NetworkManager;

    using Network.Servers.EveryBodyVsTheTeacher;

    using Utils;

    public class PlayersConnectingStateDataSender : IPlayersConnectingStateDataSender
    {
        private readonly IPlayersConnectingToTheServerState playersConnectingState;

        private readonly IServerNetworkManager networkManager;
        private readonly IEveryBodyVsTheTeacherServer server;
        
        private bool mustSentNotEnoughPlayersCommand = false;

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

            this.playersConnectingState = playersConnectingState;
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

        private void OnMainPlayerRequestedGameStart(object sender, ClientConnectionIdEventArgs args)
        {
            var mainPlayerRequestedCommand = NetworkCommandData.From<MainPlayerRequestedGameStartCommand>();
            mainPlayerRequestedCommand.AddOption("ConnectionId", args.ConnectionId.ToString());
            this.networkManager.SendClientCommand(this.server.PresenterId, mainPlayerRequestedCommand);
        }

        private void OnAudiencePlayerDisconnected(object sender, ClientConnectionIdEventArgs args)
        {
            this.SendToPresenterClientDisconnected(args.ConnectionId, false);
        }

        private void OnAudiencePlayerConnected(object sender, ClientConnectionIdEventArgs args)
        {
            this.SendToPresenterClientConnected(args.ConnectionId, false);
        }

        private void OnMainPlayerDisconnected(object sender, ClientConnectionIdEventArgs args)
        {
            this.SendToPresenterClientDisconnected(args.ConnectionId, true);

            if (this.playersConnectingState.MainPlayersConnectionIds.Count < EveryBodyVsTheTeacherServer.MinMainPlayersNeededToStartGame && 
                this.mustSentNotEnoughPlayersCommand)
            {
                var notEnoughPlayersCommand = NetworkCommandData.From<NotEnoughPlayersToStartGameCommand>();
                this.SendToMainPlayers(notEnoughPlayersCommand);
            }
        }

        private void OnMainPlayerConnected(object sender, ClientConnectionIdEventArgs args)
        {
            var connectionId = args.ConnectionId;
            var timer = TimerUtils.ExecuteWhen(
                () =>
                    {
                        return !string.IsNullOrEmpty(this.networkManager.GetClientUsername(connectionId));
                    },
                () =>
                    {
                        this.SendToPresenterClientConnected(args.ConnectionId, true);

                        if (this.playersConnectingState.MainPlayersConnectionIds.Count
                            >= EveryBodyVsTheTeacherServer.MinMainPlayersNeededToStartGame)
                        {
                            var enoughPlayersCommand = NetworkCommandData.From<EnoughPlayersToStartGameCommand>();
                            this.SendToMainPlayers(enoughPlayersCommand);
                            this.mustSentNotEnoughPlayersCommand = true;
                        }
                    });
            timer.RunOnUnityThread = true;
            timer.Start();
        }

        private void SendToMainPlayers(NetworkCommandData command)
        {
            var mainPlayersConnectionIds = this.playersConnectingState.MainPlayersConnectionIds.ToList();

            for (int i = 0; i < mainPlayersConnectionIds.Count; i++)
            {
                var connectionId = mainPlayersConnectionIds[i];
                this.networkManager.SendClientCommand(connectionId, command);
            }
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