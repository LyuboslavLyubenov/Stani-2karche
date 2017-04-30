namespace Network.Servers.EveryBodyVsTheTeacher
{

    using System;
    using System.Collections.Generic;

    using Assets.Scripts.Interfaces.States.EveryBodyVsTheTeacher.Server;
    using Assets.Scripts.States.EveryBodyVsTheTeacher.Server;

    using Commands.Server;
    
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
        private ICreatedGameInfoSender sender;

        [Inject]
        private IServerNetworkManager networkManager;
        
        [Inject]
        private PlayersConnectingToTheServerState playersConnectingToTheServerState;
    
        [Inject]
        private IRoundsSwitcher roundsSwitcher;

        [Inject]
        private StateMachine stateMachine;
        
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
            this.networkManager.CommandsManager.AddCommand(new MainPlayerConnectingCommand(this.OnMainPlayerConnecting));
            this.playersConnectingToTheServerState.OnEveryBodyRequestedGameStart += this.OnEveryBodyRequestedGameStart;
            
            this.roundsSwitcher.OnNoMoreRounds += OnNoMoreRounds;
        }

        private void OnNoMoreRounds(object sender, EventArgs args)
        {
            this.EndGame();
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

        private void OnEveryBodyRequestedGameStart(object sender, EventArgs eventArgs)
        {
            this.roundsSwitcher.SwitchToNextRound();
        }
        
        public void EndGame()
        {
            var endGameState = new EndGameState(this.networkManager);
            this.stateMachine.SetCurrentState(endGameState);
        }
    }
}