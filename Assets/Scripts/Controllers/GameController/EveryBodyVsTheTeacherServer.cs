namespace Assets.Scripts.Controllers.GameController
{

    using System;
    using System.Collections.Generic;

    using Assets.Scripts.Interfaces;
    using Assets.Scripts.Network;
    using Assets.Scripts.Network.Broadcast;
    using Assets.Scripts.Network.NetworkManagers;
    using Assets.Scripts.Network.TcpSockets;
    using Assets.Scripts.StateMachine;
    using Assets.Scripts.States.EverybodyVsTheTeacherServer;
    using Assets.Scripts.Utils.Unity;

    using Zenject;

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
        
        private CreatedGameInfoSenderService senderService = null;

        [Inject]
        private IServerNetworkManager serverNetworkManager;

        private readonly SimpleFiniteStateMachine stateMachine = new SimpleFiniteStateMachine();

        [Inject]
        private PlayersConnectingToTheServerState playersConnectingToTheServerState;

        private HashSet<int> mainPlayersConnectionIds;
        private HashSet<int> audiencePlayersconnectionIds;

        void Start()
        {
            this.senderService = new CreatedGameInfoSenderService(
                new SimpleTcpClient(),
                new SimpleTcpServer(7772),
                GameInfoFactory.Instance,
                serverNetworkManager,
                this);
            

            this.stateMachine.SetCurrentState(this.playersConnectingToTheServerState);
        }

        public void StartGame()
        {
            //TODO

            this.mainPlayersConnectionIds = new HashSet<int>(this.playersConnectingToTheServerState.MainPlayersConnectionIds);
            this.audiencePlayersconnectionIds = new HashSet<int>(this.playersConnectingToTheServerState.AudiencePlayersConnectionIds);

            //this.stateMachine.SetCurrentState();
            this.StartedGame = true;
        }

        public void EndGame()
        {
            throw new NotImplementedException();
        }
    }
}