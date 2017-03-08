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
                return this.audiencePlayersconnectionIds;
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
        
        private readonly StateMachine StateMachine = new StateMachine();

        [Inject]
        private PlayersConnectingToTheServerState playersConnectingToTheServerState;

        private HashSet<int> mainPlayersConnectionIds = new HashSet<int>();
        private HashSet<int> audiencePlayersconnectionIds = new HashSet<int>();

        void Start()
        {
        }

        public void StartGame()
        {
            throw new NotImplementedException();
        }

        public void EndGame()
        {
            throw new NotImplementedException();
        }
    }
}