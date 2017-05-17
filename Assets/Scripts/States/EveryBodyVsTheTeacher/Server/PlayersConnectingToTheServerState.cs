using EveryBodyVsTheTeacherServer = Network.Servers.EveryBodyVsTheTeacher.EveryBodyVsTheTeacherServer;

namespace States.EveryBodyVsTheTeacher.Server
{

    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;

    using Commands.EveryBodyVsTheTeacher;
    using Commands.EveryBodyVsTheTeacher.PlayersConnectingState;
    using Commands.Server;

    using EventArgs;

    using Extensions;

    using Interfaces.Network.EveryBodyVsTheTeacher.States;
    using Interfaces.Network.NetworkManager;

    using StateMachine;

    using Utils;

    using Zenject.Source.Usage;

    public class PlayersConnectingToTheServerState : IPlayersConnectingToTheServerState
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

        public event EventHandler<ClientConnectionIdEventArgs> OnMainPlayerConnected = delegate
            {
            };
        public event EventHandler<ClientConnectionIdEventArgs> OnMainPlayerDisconnected = delegate
            {
            };

        public event EventHandler OnEveryBodyRequestedGameStart = delegate
            {
            };
        public event EventHandler<ClientConnectionIdEventArgs> OnMainPlayerRequestedGameStart = delegate
            {   
            };

        public event EventHandler<ClientConnectionIdEventArgs> OnAudiencePlayerConnected = delegate
            {
            };
        public event EventHandler<ClientConnectionIdEventArgs> OnAudiencePlayerDisconnected = delegate
            {
            };

        private readonly HashSet<int> audiencePlayersConnectionIds = new HashSet<int>();
        private readonly HashSet<int> mainPlayersConnectionsIds = new HashSet<int>();
        private readonly HashSet<int> playersRequestingGameStartIds = new HashSet<int>();

        //---
        [Inject]
        private IServerNetworkManager networkManager;
        //---

        private readonly StartGameRequestCommand startGameRequestCommand = new StartGameRequestCommand();

        private void OnPlayerDisconnectedFromServer(object sender, ClientConnectionIdEventArgs args)
        {
            if (this.mainPlayersConnectionsIds.Count < EveryBodyVsTheTeacherServer.MinMainPlayersNeededToStartGame &&
                this.networkManager.CommandsManager.Exists<StartGameRequestCommand>())
            {
                this.networkManager.CommandsManager.RemoveCommand<StartGameRequestCommand>();
            }

            if (this.playersRequestingGameStartIds.Contains(args.ConnectionId))
            {
                this.playersRequestingGameStartIds.Remove(args.ConnectionId);
            }

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

        private void OnPlayerConnectedToServer(object sender, ClientConnectionIdEventArgs args)
        {
            var connectionId = args.ConnectionId;
            var timer = TimerUtils.ExecuteAfter(0.5f, () => this.CheckIsClientJoinedAudienceOrMainPlayers(connectionId));
            timer.RunOnUnityThread = true;
            timer.AutoDispose = true;
            timer.Start();
        }

        private void OnMainPlayerConnecting(int connectionId)
        {
            if (this.mainPlayersConnectionsIds.Count < EveryBodyVsTheTeacherServer.MaxMainPlayersNeededToStartGame)
            {
                this.mainPlayersConnectionsIds.Add(connectionId);
                this.OnMainPlayerConnected(this, new ClientConnectionIdEventArgs(connectionId));
            }
            else
            {
                //TODO: too much players
                this.networkManager.KickPlayer(connectionId, "EverybodyVsTheTeacher/CannotConnectAsMainPlayer/PlacesAreFull");
            }

            if (this.mainPlayersConnectionsIds.Count >= EveryBodyVsTheTeacherServer.MinMainPlayersNeededToStartGame &&
                !this.networkManager.CommandsManager.Exists<StartGameRequestCommand>())
            {
                this.networkManager.CommandsManager.AddCommand(this.startGameRequestCommand);
            }
        }

        private void CheckIsClientJoinedAudienceOrMainPlayers(int connectionId)
        {
            if (this.mainPlayersConnectionsIds.Contains(connectionId))
            {
                return;
            }

            this.audiencePlayersConnectionIds.Add(connectionId);
            this.OnAudiencePlayerConnected(this, new ClientConnectionIdEventArgs(connectionId));
        }

        private void OnMainPlayerRequestGameStart(object sender, DummyCommandReceivedDataEventArgs dummyCommandReceivedDataEventArgs)
        {
            var connectionId = dummyCommandReceivedDataEventArgs.CommandsOptionsValues["ConnectionId"].ConvertTo<int>();

            if (this.playersRequestingGameStartIds.Contains(connectionId))
            {
                return;
            }

            this.playersRequestingGameStartIds.Add(connectionId);
            
            this.OnMainPlayerRequestedGameStart(this, new ClientConnectionIdEventArgs(connectionId));

            if (this.mainPlayersConnectionsIds.All(this.playersRequestingGameStartIds.Contains))
            {
                this.OnEveryBodyRequestedGameStart(this, EventArgs.Empty);
            }
        }

        private void AttachEventHandlers()
        {
            this.networkManager.OnClientConnected += this.OnPlayerConnectedToServer;
            this.networkManager.OnClientDisconnected += this.OnPlayerDisconnectedFromServer;

            this.startGameRequestCommand.OnExecuted += this.OnMainPlayerRequestGameStart;
        }

        private void DetachEventHandlers()
        {
            this.networkManager.OnClientConnected -= this.OnPlayerConnectedToServer;
            this.networkManager.OnClientDisconnected -= this.OnPlayerDisconnectedFromServer;
            this.startGameRequestCommand.OnExecuted -= this.OnMainPlayerRequestGameStart;
        }

        private void ClearSubscriptions()
        {
            this.OnAudiencePlayerConnected = delegate
            {
            };
            this.OnAudiencePlayerDisconnected = delegate
            {
            };
            this.OnMainPlayerConnected = delegate
            {
            };
            this.OnMainPlayerDisconnected = delegate
            {
            };
        }

        public void OnStateEnter(StateMachine stateMachine)
        {
            this.AttachEventHandlers();
            this.networkManager.CommandsManager.AddCommand(new MainPlayerConnectingCommand(this.OnMainPlayerConnecting));
        }

        public void OnStateExit(StateMachine stateMachine)
        {
            this.DetachEventHandlers();
            this.ClearSubscriptions();

            this.networkManager.CommandsManager.RemoveCommand<MainPlayerConnectingCommand>();

            if (this.networkManager.CommandsManager.Exists<StartGameRequestCommand>())
            {
                this.networkManager.CommandsManager.RemoveCommand<StartGameRequestCommand>();
            }

            this.mainPlayersConnectionsIds.Clear();
            this.audiencePlayersConnectionIds.Clear();
        }
    }
}