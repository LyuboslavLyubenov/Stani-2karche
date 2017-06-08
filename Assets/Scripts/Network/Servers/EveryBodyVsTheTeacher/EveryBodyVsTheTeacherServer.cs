namespace Network.Servers.EveryBodyVsTheTeacher
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Assets.Scripts.Commands.EveryBodyVsTheTeacher;
    using Assets.Scripts.Commands.EveryBodyVsTheTeacher.Shared;
    using Assets.Scripts.Extensions;
    using Assets.Scripts.Interfaces.States.EveryBodyVsTheTeacher.Server;
    using Assets.Scripts.Network.EveryBodyVsTheTeacher;
    using Assets.Scripts.States.EveryBodyVsTheTeacher.Server;

    using Commands;
    using Commands.Server;

    using EventArgs;

    using Interfaces.GameData;
    using Interfaces.Network;
    using Interfaces.Network.EveryBodyVsTheTeacher;
    using Interfaces.Network.EveryBodyVsTheTeacher.States;
    using Interfaces.Network.NetworkManager;

    using StateMachine;

    using Utils.Unity;

    using Zenject.Source.Usage;

    public class EveryBodyVsTheTeacherServer : ExtendedMonoBehaviour, IEveryBodyVsTheTeacherServer
    {
        public const int MinMainPlayersNeededToStartGame = 6;
        public const int MaxMainPlayersNeededToStartGame = 10;

        public event EventHandler OnGameOver = delegate
            {
            };

        [Inject]
        private ICreatedGameInfoSender gameInfoSender;

        [Inject]
        private IGameDataIterator gameDataIterator;

        [Inject]
        private IServerNetworkManager networkManager;

        [Inject]
        private IPlayersConnectingToTheServerState playersConnectingToTheServerState;

        [Inject]
        private IRoundsSwitcher roundsSwitcher;

        [Inject]
        private StateMachine stateMachine;

        [Inject]
        private RoundsSwitcherEventsNotifier roundsSwitcherEventsNotifier;

        [Inject]
        private IPlayersConnectingStateDataSender playersConnectingStateDataSender;

        private HashSet<int> mainPlayersConnectionIds = new HashSet<int>();

        public bool IsGameOver
        {
            get;
            private set;
        }

        public IEnumerable<int> MainPlayersConnectionIds
        {
            get
            {
                return this.mainPlayersConnectionIds;
            }
        }

        public bool StartedGame
        {
            get;
            private set;
        }

        public int PresenterId
        {
            get;
            private set;
        }

        void Start()
        {
            this.networkManager.OnClientDisconnected += this.OnClientDisconneted;
            this.roundsSwitcher.OnMustEndGame += this.OnMustEndGame;
            this.roundsSwitcher.OnNoMoreRounds += this.OnNoMoreRounds;
            this.playersConnectingToTheServerState.OnEveryBodyRequestedGameStart += this.OnEveryBodyRequestedGameStart;

            this.networkManager.CommandsManager.AddCommand(new MainPlayerConnectingCommand(this.OnMainPlayerConnecting));
            this.networkManager.CommandsManager.AddCommand(new PresenterConnectingCommand(this.OnPresenterConnecting));

            this.stateMachine.SetCurrentState(this.playersConnectingToTheServerState);
        }

        private void OnClientDisconneted(object sender, ClientConnectionIdEventArgs args)
        {
            if (args.ConnectionId == this.PresenterId)
            {
                this.PresenterId = 0;
            }
        }

        private void OnPresenterConnecting(int connectionId)
        {
            if (this.PresenterId == connectionId)
            {
                return;
            }

            if (this.PresenterId > 0)
            {
                this.networkManager.KickPlayer(connectionId, "Presenter already connected");//TODO: Transate
                return;
            }
            
            this.PresenterId = connectionId;
            this.SendCurrentStateToPresenter();
        }

        private void SendCurrentStateToPresenter()
        {
            var stateName = string.Empty;

            if (this.stateMachine.CurrentState == this.playersConnectingToTheServerState)
            {
                stateName = "PlayersConnectingState";
            }
            else if (this.stateMachine.CurrentState.IsImplemetingInterface(typeof(IRoundState)))
            {
                stateName = "PlayingState";
            }
            else
            {
                return;
            }

            var activateStateCommand = new NetworkCommandData("Activate" + stateName);
            this.networkManager.SendClientCommand(this.PresenterId, activateStateCommand);
        }

        private void OnMainPlayerConnecting(int connectionId)
        {
            if (this.stateMachine.CurrentState == this.playersConnectingToTheServerState ||
                this.mainPlayersConnectionIds.Contains(connectionId))
            {
                return;
            }
            
            this.networkManager.KickPlayer(connectionId);
        }

        private void OnMustEndGame(object sender, EventArgs args)
        {
            this.EndGame();
        }

        private void OnNoMoreRounds(object sender, EventArgs args)
        {
            this.EndGame();
        }

        private void SendMainPlayersCommand(NetworkCommandData command)
        {
            for (int i = 0; i < this.mainPlayersConnectionIds.Count; i++)
            {
                var connectionId = this.mainPlayersConnectionIds.Skip(i).First();
                this.networkManager.SendClientCommand(connectionId, command);
            }
        }

        private void OnEveryBodyRequestedGameStart(object sender, EventArgs args)
        {
            this.mainPlayersConnectionIds =
                new HashSet<int>(this.playersConnectingToTheServerState.MainPlayersConnectionIds);

            var startedGameCommand = NetworkCommandData.From<GameStartedCommand>();
            this.SendMainPlayersCommand(startedGameCommand);

            this.roundsSwitcher.SwitchToNextRound();
            
            this.StartedGame = true;
        }

        public void EndGame()
        {
            var endGameState = new EndGameState(this.networkManager, this.gameDataIterator);
            this.stateMachine.SetCurrentState(endGameState);

            this.IsGameOver = true;
            this.OnGameOver(this, EventArgs.Empty);
        }
    }
}