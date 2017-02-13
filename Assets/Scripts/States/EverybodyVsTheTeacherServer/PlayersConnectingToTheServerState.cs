namespace Assets.Scripts.States.EverybodyVsTheTeacherServer
{

    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;

    using Assets.Scripts.Commands.Server;
    using Assets.Scripts.Controllers.GameController;
    using Assets.Scripts.EventArgs;
    using Assets.Scripts.Interfaces;
    using Assets.Scripts.StateMachine;
    using Assets.Scripts.Utils;

    public class PlayersConnectingToTheServerState : IState
    {

        public ReadOnlyCollection<int> MainPlayersConnectionIds
        {
            get
            {
                return new ReadOnlyCollection<int>(this.mainPlayersConnectionsIds.ToArray());
            }
        }

        public ReadOnlyCollection<int> AudiencePlayersConnectionIds
        {
            get
            {
                return new ReadOnlyCollection<int>(this.audiencePlayersConnectionIds.ToArray());
            }
        }

        public event EventHandler<ClientConnectionDataEventArgs> OnMainPlayerConnected = delegate
            {
            };

        public event EventHandler<ClientConnectionDataEventArgs> OnMainPlayerDisconnected = delegate
            {
            };
        public event EventHandler<ClientConnectionDataEventArgs> OnAudiencePlayerConnected = delegate
            {
            };
        public event EventHandler<ClientConnectionDataEventArgs> OnAudiencePlayerDisconnected = delegate
            {
            };
        
        private readonly HashSet<int> audiencePlayersConnectionIds = new HashSet<int>();
        private readonly HashSet<int> mainPlayersConnectionsIds = new HashSet<int>();
        
        private readonly IServerNetworkManager networkManager;
        
        public PlayersConnectingToTheServerState(IServerNetworkManager networkManager)
        {
            if (networkManager == null)
            {
                throw new ArgumentNullException("networkManager");
            }

            this.networkManager = networkManager;
        }

        private void OnPlayerDisconnectedFromServer(object sender, ClientConnectionDataEventArgs args)
        {
            if (this.mainPlayersConnectionsIds.Contains(args.ConnectionId))
            {
                this.mainPlayersConnectionsIds.Remove(args.ConnectionId);
                this.OnMainPlayerDisconnected(this, args);
                return;
            }

            if (this.audiencePlayersConnectionIds.Contains(args.ConnectionId))
            {
                this.audiencePlayersConnectionIds.Remove(args.ConnectionId);
                this.OnAudiencePlayerDisconnected(this, args);
                return;
            }
        }

        private void OnPlayerConnectedToServer(object sender, ClientConnectionDataEventArgs args)
        {
            var connectionId = args.ConnectionId;
            var timer = TimerUtils.ExecuteAfter(1f, () => this.CheckIsClientJoinedAudienceOrMainPlayers(connectionId));
            timer.RunOnUnityThread = true;
            timer.AutoDispose = true;
        }

        private void OnMainPlayerConnecting(int connectionId)
        {
            if (this.mainPlayersConnectionsIds.Count < EveryBodyVsTheTeacherServer.MaxMainPlayersNeededToStartGame)
            {
                this.mainPlayersConnectionsIds.Add(connectionId);
                this.OnMainPlayerConnected(this, new ClientConnectionDataEventArgs(connectionId));
            }
            else
            {
                //TODO: too much players
                this.networkManager.KickPlayer(connectionId, "EverybodyVsTheTeacher/CannotConnectAsMainPlayer/PlacesAreFull");
            }
        }

        private void CheckIsClientJoinedAudienceOrMainPlayers(int connectionId)
        {
            if (this.mainPlayersConnectionsIds.Contains(connectionId))
            {
                return;
            }

            this.audiencePlayersConnectionIds.Add(connectionId);
            this.OnAudiencePlayerConnected(this, new ClientConnectionDataEventArgs(connectionId));
        }

        private void AttachEventHandlers()
        {
            this.networkManager.OnClientConnected += this.OnPlayerConnectedToServer;
            this.networkManager.OnClientDisconnected += this.OnPlayerDisconnectedFromServer;
        }

        private void DetachEventHandlers()
        {
            this.networkManager.OnClientConnected -= this.OnPlayerConnectedToServer;
            this.networkManager.OnClientDisconnected -= this.OnPlayerDisconnectedFromServer;
        }
        

        public void OnStateEnter(SimpleFiniteStateMachine stateMachine)
        {
            this.AttachEventHandlers();

            this.networkManager.CommandsManager.AddCommand(new MainPlayerConnectingCommand(this.OnMainPlayerConnecting));
        }
        
        public void OnStateExit(SimpleFiniteStateMachine stateMachine)
        {
            this.DetachEventHandlers();

            this.networkManager.CommandsManager.RemoveCommand<MainPlayerConnectingCommand>();

            this.mainPlayersConnectionsIds.Clear();
            this.audiencePlayersConnectionIds.Clear();
        }
    }
}