using EveryBodyVsTheTeacherServer = Network.Servers.EveryBodyVsTheTeacher.EveryBodyVsTheTeacherServer;

namespace States.EveryBodyVsTheTeacher.Server
{

    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;

    using Assets.Scripts.Commands.EveryBodyVsTheTeacher;
    using Assets.Scripts.Extensions;

    using Commands;
    using Commands.EveryBodyVsTheTeacher;
    using Commands.EveryBodyVsTheTeacher.PlayersConnectingState;
    using Commands.Server;

    using EventArgs;

    using Interfaces.Network.EveryBodyVsTheTeacher.States;
    using Interfaces.Network.NetworkManager;

    using StateMachine;

    using States.EveryBodyVsTheTeacher.Shared;

    using Utils;

    using Zenject;

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

        private readonly MainPlayerConnectingCommand mainPlayerConnectingCommand;
        private readonly StartGameRequestCommand startGameRequestCommand = new StartGameRequestCommand();
        private readonly PresenterConnectingCommand presenterConnectingCommand;

        private int presenterConnectionId = 0;

        private bool areAllMainPlayersRequestedGameStart
        {
            get
            {
                return 
                    this.mainPlayersConnectionsIds.Any() &&
                    this.mainPlayersConnectionsIds.All(this.playersRequestingGameStartIds.Contains);
            }
        }

        //---
        private IServerNetworkManager networkManager;
        //---
        
        [Inject]
        public PlayersConnectingToTheServerState(IServerNetworkManager networkManager)
        {
            if (networkManager == null)
            {
                throw new ArgumentNullException("networkManager");
            }

            this.networkManager = networkManager;
            this.mainPlayerConnectingCommand = new MainPlayerConnectingCommand(this.OnMainPlayerConnecting);
            this.presenterConnectingCommand = new PresenterConnectingCommand(this.OnPresenterConnecting);
        }
        
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
            }
            else if (this.audiencePlayersConnectionIds.Contains(args.ConnectionId))
            {
                this.audiencePlayersConnectionIds.Remove(args.ConnectionId);
                this.OnAudiencePlayerDisconnected(this, args);
            }
            
            if (this.areAllMainPlayersRequestedGameStart)
            {
                this.OnEveryBodyRequestedGameStart(this, EventArgs.Empty);
            }
        }

        private void OnPlayerSetUsername(object sender, ConnectedClientDataEventArgs args)
        {
            var connectionId = args.ClientData.ConnectionId;
            var timer = TimerUtils.ExecuteAfter(0.5f, () => this.CheckIsClientJoinedAudienceOrMainPlayers(connectionId));
            timer.RunOnUnityThread = true;
            timer.AutoDispose = true;
            timer.Start();
        }

        private void OnMainPlayerConnecting(int connectionId)
        {
            if (this.audiencePlayersConnectionIds.Contains(connectionId))
            {
                this.networkManager.KickPlayer(connectionId);
                return;
            }

            if (this.mainPlayersConnectionsIds.Count <= EveryBodyVsTheTeacherServer.MaxMainPlayersNeededToStartGame)
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
            if (this.mainPlayersConnectionsIds.Contains(connectionId) || this.presenterConnectionId == connectionId)
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

            if (this.areAllMainPlayersRequestedGameStart)
            {
                this.OnEveryBodyRequestedGameStart(this, EventArgs.Empty);
            }
        }

        private void OnPresenterConnecting(int connectionId)
        {
            this.presenterConnectionId = connectionId;
            this.SendAllConnectedMainPlayersToPresenter();
            this.SendAllConnectedAudiencePlayersToPresenter();
            this.SendAllRequestsForGameStartToPresenter();
        }

        private void SendAllConnectedMainPlayersToPresenter()
        {
            this.SendAllConnectedPlayersOfTypeToPresenter(ClientType.MainPlayer, this.mainPlayersConnectionsIds.ToList());
        }

        private void SendAllConnectedAudiencePlayersToPresenter()
        {
            this.SendAllConnectedPlayersOfTypeToPresenter(ClientType.Audience, this.audiencePlayersConnectionIds.ToList());
        }

        private void SendAllConnectedPlayersOfTypeToPresenter(ClientType clientType, IList<int> connectionIds)
        {
            if (clientType == ClientType.Presenter)
            {
                throw new InvalidOperationException();
            }

            var commandType = clientType == ClientType.MainPlayer
                                  ? typeof(MainPlayerConnectedCommand)
                                  : typeof(AudiencePlayerConnectedCommand);
            var commandName = commandType.Name.Replace("Command", "");

            for (int i = 0; i < connectionIds.Count; i++)
            {
                var connectionId = connectionIds[i];
                var username = this.networkManager.GetClientUsername(connectionId);
                var command = new NetworkCommandData(commandName);
                command.AddOption("ConnectionId", connectionId.ToString());
                command.AddOption("Username", username);
                this.networkManager.SendClientCommand(this.presenterConnectionId, command);
            }
        }

        private void SendAllRequestsForGameStartToPresenter()
        {
            for (int i = 0; i < this.playersRequestingGameStartIds.Count; i++)
            {
                var connectionId = this.playersRequestingGameStartIds.Skip(i).First();
                var command = NetworkCommandData.From<StartGameRequestCommand>();
                command.AddOption("ConnectionId", connectionId.ToString());
                this.networkManager.SendClientCommand(this.presenterConnectionId, command);
            }
        }

        private void AttachEventHandlers()
        {
            this.networkManager.OnClientSetUsername += this.OnPlayerSetUsername;
            this.networkManager.OnClientDisconnected += this.OnPlayerDisconnectedFromServer;

            this.startGameRequestCommand.OnExecuted += this.OnMainPlayerRequestGameStart;
        }

        private void DetachEventHandlers()
        {
            this.networkManager.OnClientSetUsername -= this.OnPlayerSetUsername;
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
            this.networkManager.CommandsManager.AddCommand(this.mainPlayerConnectingCommand);
            this.networkManager.CommandsManager.AddCommand(this.presenterConnectingCommand);
        }

        public void OnStateExit(StateMachine stateMachine)
        {
            this.DetachEventHandlers();
            this.ClearSubscriptions();

            this.networkManager.CommandsManager.RemoveCommand(this.mainPlayerConnectingCommand);

            if (this.networkManager.CommandsManager.Exists(this.startGameRequestCommand))
            {
                this.networkManager.CommandsManager.RemoveCommand(this.startGameRequestCommand);
            }

            this.networkManager.CommandsManager.RemoveCommand(this.presenterConnectingCommand);

            this.mainPlayersConnectionsIds.Clear();
            this.audiencePlayersConnectionIds.Clear();
        }
    }
}