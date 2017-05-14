namespace Network.Servers.EveryBodyVsTheTeacher
{
    using System;
    using System.Collections.Generic;

    using Assets.Scripts.Commands.EveryBodyVsTheTeacher;
    using Assets.Scripts.Interfaces.States.EveryBodyVsTheTeacher.Server;
    using Assets.Scripts.Network.EveryBodyVsTheTeacher;
    using Assets.Scripts.States.EveryBodyVsTheTeacher.Server;

    using Commands;
    using Commands.Server;

    using EventArgs;

    using Interfaces.GameData;
    using Interfaces.Network;
    using Interfaces.Network.NetworkManager;
    
    using StateMachine;

    using States.EveryBodyVsTheTeacher.Server;
    
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
        private PlayersConnectingToTheServerState playersConnectingToTheServerState;
    
        [Inject]
        private IRoundsSwitcher roundsSwitcher;

        [Inject]
        private StateMachine stateMachine;

        [Inject]
        private RoundsSwitcherEventsNotifier roundsSwitcherEventsNotifier;
        
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
            this.networkManager.OnClientConnected += this.OnClientConnectedToServer;
            this.roundsSwitcher.OnMustEndGame += this.OnMustEndGame;
            this.roundsSwitcher.OnNoMoreRounds += this.OnNoMoreRounds;
            this.playersConnectingToTheServerState.OnEveryBodyRequestedGameStart += this.OnEveryBodyRequestedGameStart;

            this.networkManager.CommandsManager.AddCommand(new MainPlayerConnectingCommand(this.OnMainPlayerConnecting));            
            this.networkManager.CommandsManager.AddCommand(new PresenterConnectingCommand(this.OnPresenterConnecting));
        }

        private void OnClientConnectedToServer(object sender, ClientConnectionIdEventArgs args)
        {
            if (this.PresenterId <= 0)
            {
                this.networkManager.KickPlayer(args.ConnectionId, "Must connect presenter first");//TODO: translate
            }
        }

        private void OnPresenterConnecting(int connectionId)
        {
            if (this.StartedGame)
            {
                if (this.PresenterId != connectionId)
                {
                    this.networkManager.KickPlayer(connectionId, "You are not presenter");//TODO: Translate
                }

                return;
            }

            if (this.PresenterId > 0)
            {
                this.networkManager.KickPlayer(connectionId, "Presenter already connected");//TODO: Transate
                return;
            }
            
            this.PresenterId = connectionId;
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
        
        private void OnEveryBodyRequestedGameStart(object sender, EventArgs args)
        {
            this.roundsSwitcher.SwitchToNextRound();
            this.networkManager.SendClientCommand(this.PresenterId, new NetworkCommandData("GameStarted"));
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