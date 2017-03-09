namespace Assets.Scripts.Controllers.GameController
{
    using System;
    using System.Collections.Generic;

    using Assets.Scripts.Controllers.EveryBodyVsTheTeacher.States.Server;
    using Assets.Scripts.Interfaces;
    using Assets.Scripts.Interfaces.Network.NetworkManager;
    using Assets.Scripts.StateMachine;
    using Assets.Scripts.Utils.Unity;
    using Assets.Zenject.Source.Usage;

    public class EveryBodyVsTheTeacherServer : ExtendedMonoBehaviour, IGameServer
    {
        public const int MinMainPlayersNeededToStartGame = 6;
        public const int MaxMainPlayersNeededToStartGame = 10;

        public event EventHandler OnGameOver = delegate
            {
            };

        public bool IsGameOver
        {
            get; private set;
        }

        public IEnumerable<int> MainPlayersConnectionIds
        {
            get
            {
                return this.mainPlayersConnectionIds;
            }
        }

        public IEnumerable<int> AudiencePlayersConnectionIds
        {
            get
            {
                return this.audiencePlayersConnectionIds;
            }
        }

        public bool StartedGame
        {
            get; private set;
        }
        
        [Inject]
        private ICreatedGameInfoSender sender;

        [Inject]
        private IServerNetworkManager serverNetworkManager;

        [Inject]
        private PlayersConnectingToTheServerState playersConnectingToTheServerState;

        [Inject]
        private GameRunningState gameRunningState;

        private readonly StateMachine stateMachine = new StateMachine();
        
        private HashSet<int> mainPlayersConnectionIds = new HashSet<int>();
        private HashSet<int> audiencePlayersConnectionIds = new HashSet<int>();

        void Start()
        {
            this.playersConnectingToTheServerState.OnEveryBodyRequestedGameStart += this.OnEveryBodyRequestedGameStart;
        }

        private void OnEveryBodyRequestedGameStart(object sender, EventArgs eventArgs)
        {
            this.mainPlayersConnectionIds = 
                new HashSet<int>(this.playersConnectingToTheServerState.MainPlayersConnectionIds);
            this.audiencePlayersConnectionIds =
                new HashSet<int>(this.playersConnectingToTheServerState.AudiencePlayersConnectionIds);

            this.stateMachine.SetCurrentState(this.gameRunningState);
        }
        
        public void EndGame()
        {
            throw new NotImplementedException();
        }
    }
}