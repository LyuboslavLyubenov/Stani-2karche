namespace Network.Servers.EveryBodyVsTheTeacher
{

    using System;
    using System.Collections.Generic;

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
        private IServerNetworkManager serverNetworkManager;

        [Inject]
        private PlayersConnectingToTheServerState playersConnectingToTheServerState;

        [Inject]
        private FirstRoundState firstRoundState;

        private JokersData jokers;

        private readonly StateMachine stateMachine = new StateMachine();

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
            this.serverNetworkManager.CommandsManager.AddCommand(new MainPlayerConnectingCommand(this.OnMainPlayerConnecting));
            this.playersConnectingToTheServerState.OnEveryBodyRequestedGameStart += this.OnEveryBodyRequestedGameStart;
        }

        private void OnMainPlayerConnecting(int connectionId)
        {
            if (this.stateMachine.CurrentState == this.playersConnectingToTheServerState ||
                this.mainPlayersConnectionIds.Contains(connectionId))
            {
                return;
            }

            this.serverNetworkManager.KickPlayer(connectionId);
        }

        private void OnEveryBodyRequestedGameStart(object sender, EventArgs eventArgs)
        {
            this.stateMachine.SetCurrentState(this.firstRoundState);
        }

        public void EndGame()
        {

        }
    }
}