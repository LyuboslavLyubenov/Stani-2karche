namespace Network.Servers.EveryBodyVsTheTeacher
{
    using MainPlayerConnectingCommand = Assets.Scripts.Commands.Server.EveryBodyVsTheTeacher.MainPlayerConnectingCommand;

    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Assets.Scripts.Commands.EveryBodyVsTheTeacher;
    using Assets.Scripts.Commands.EveryBodyVsTheTeacher.Shared;
    using Assets.Scripts.Extensions;
    using Assets.Scripts.Interfaces.Network;
    using Assets.Scripts.Interfaces.States.EveryBodyVsTheTeacher.Server;
    using Assets.Scripts.Network;
    using Assets.Scripts.Network.EveryBodyVsTheTeacher;
    using Assets.Scripts.Network.GameInfo.New;
    using Assets.Scripts.Network.JokersData.EveryBodyVsTheTeacher;
    using Assets.Scripts.States.EveryBodyVsTheTeacher.Server;
    using Assets.Scripts.States.EveryBodyVsTheTeacher.Server.Rounds;

    using Commands;

    using EventArgs;

    using Interfaces.GameData;
    using Interfaces.Network;
    using Interfaces.Network.EveryBodyVsTheTeacher;
    using Interfaces.Network.EveryBodyVsTheTeacher.States;
    using Interfaces.Network.NetworkManager;

    using Network.Broadcast;
    using Network.EveryBodyVsTheTeacher.PlayersConnectingState;
    using Network.NetworkManagers;

    using StateMachine;

    using States.EveryBodyVsTheTeacher.Server;

    using Utils.Unity;

    using Zenject;

    public class EveryBodyVsTheTeacherServer : ExtendedMonoBehaviour, IEveryBodyVsTheTeacherServer
    {

#if DEVELOPMENT_BUILD || UNITY_EDITOR
        public const int MinMainPlayersNeededToStartGame = 2;
        public const int MaxMainPlayersNeededToStartGame = 8;
#else
        public const int MinMainPlayersNeededToStartGame = 7;
        public const int MaxMainPlayersNeededToStartGame = 8;
#endif

        public event EventHandler OnGameOver = delegate
            {
            };

        [Inject]
        private IGameDataIterator gameDataIterator;
        
        [Inject]
        private IGameDataExtractor gameDataExtractor;

        [Inject]
        private JokersData jokersData;

        [Inject]
        private StateMachine stateMachine;


        private JokersDataSender jokersDataSender;

        private IPlayersConnectingToTheServerState playersConnectingToTheServerState;

        private IRoundsSwitcher roundsSwitcher;

        private RoundsSwitcherEventsNotifier roundsSwitcherEventsNotifier;
        
        private ICreatedGameInfoSender gameInfoSender;
        private IPlayersConnectingStateDataSender playersConnectingStateDataSender;
        
        private ISecondsRemainingUICommandsSender secondsRemainingUiCommandsSender;
        private IQuestionsRemainingCommandsSender questionsRemainingCommandsSender;
        private IMistakesRemainingCommandsSender mistakesRemainingCommandsSender;

        private LANServerOnlineBroadcastService broadcastService = new LANServerOnlineBroadcastService();
        
        private HashSet<int> mainPlayersConnectionIds = new HashSet<int>();
        private readonly HashSet<int> surrenderedMainPlayersConnectionIds = new HashSet<int>();

        public bool IsGameOver
        {
            get;
            private set;
        }

        public IEnumerable<int> ConnectedMainPlayersConnectionIds
        {
            get
            {
                return ServerNetworkManager.Instance.ConnectedClientsConnectionId.Intersect(this.mainPlayersConnectionIds);
            }
        }

        public IEnumerable<int> MainPlayersConnectionIds
        {
            get
            {
                return this.mainPlayersConnectionIds;
            }
        }

        public IEnumerable<int> SurrenderedMainPlayersConnectionIds
        {
            get
            {
                return this.surrenderedMainPlayersConnectionIds;
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
            var networkManager = ServerNetworkManager.Instance;

            this.playersConnectingToTheServerState = new PlayersConnectingToTheServerState(ServerNetworkManager.Instance);
            this.gameInfoSender = new CreatedGameInfoSender(networkManager, this);

            var rounds = this.ConfigureRounds();
            this.roundsSwitcher = this.BuildRoundsSwitcher(rounds);
            
            this.roundsSwitcherEventsNotifier =
                new RoundsSwitcherEventsNotifier(networkManager, this, this.roundsSwitcher);

            this.playersConnectingStateDataSender = 
                new PlayersConnectingStateDataSender(
                    this.playersConnectingToTheServerState,
                    ServerNetworkManager.Instance,
                    this);

            this.secondsRemainingUiCommandsSender = 
                new SecondsRemainingUICommandsSender(ServerNetworkManager.Instance, this);
            this.questionsRemainingCommandsSender = 
                new QuestionsRemainingUICommandsSender(
                    ServerNetworkManager.Instance, 
                    this, 
                    this.gameDataIterator);
            this.mistakesRemainingCommandsSender = 
                new MistakesRemainingCommandsSender(
                    ServerNetworkManager.Instance,
                    this,
                    this.roundsSwitcher,
                    this.gameDataIterator,
                    this.stateMachine);

            this.jokersDataSender = 
                new JokersDataSender(this.jokersData, networkManager, this);

            networkManager.CommandsManager.AddCommand(
                new MainPlayerConnectingCommand(networkManager, this, this.OnMainPlayerConnecting));
            networkManager.CommandsManager.AddCommand(
                new PresenterConnectingCommand(this.OnPresenterConnecting));
            networkManager.CommandsManager.AddCommand(
                new SurrenderCommand(networkManager, this, this.gameDataIterator));

            networkManager.OnClientDisconnected += this.OnClientDisconneted;
            this.roundsSwitcher.OnMustEndGame += this.OnMustEndGame;
            this.roundsSwitcher.OnNoMoreRounds += this.OnNoMoreRounds;
            this.playersConnectingToTheServerState.OnEveryBodyRequestedGameStart += this.OnEveryBodyRequestedGameStart;

            this.stateMachine.SetCurrentState(this.playersConnectingToTheServerState);
        }
        
        private IRoundState[] ConfigureRounds()
        {
            var networkManager = ServerNetworkManager.Instance;
            var answersCollector = new VoteResultForAnswerForCurrentQuestionCollector(this, ServerNetworkManager.Instance, gameDataIterator);
            var firstRoundBuilder = new FirstRoundState.Builder
                                    {
                                        GameDataExtractor = gameDataExtractor,
                                        Server = this,
                                        CurrentQuestionAnswersCollector = answersCollector,
                                        GameDataIterator = gameDataIterator,
                                        JokersData = jokersData,
                                        ServerNetworkManager = networkManager
                                    };
            var firstRound = firstRoundBuilder.Build();
            var secondRoundBuilder = new SecondRoundState.Builder()
                                     {
                                         CurrentQuestionAnswersCollector = answersCollector,
                                         GameDataIterator = gameDataIterator,
                                         JokersData = jokersData,
                                         Server = this,
                                         ServerNetworkManager = networkManager
                                     };
            var secondRound = secondRoundBuilder.Build();
            var thirdRoundBuilder = new ThirdRoundState.Builder()
                                    {
                                        CurrentQuestionAnswersCollector = answersCollector,
                                        GameDataIterator = gameDataIterator,
                                        JokersData = jokersData,
                                        Server = this,
                                        ServerNetworkManager = networkManager
                                    };
            var thirdRound = thirdRoundBuilder.Build();

            return new IRoundState[]
                   {
                       firstRound,
                       secondRound,
                       thirdRound
                   };
        }

        private IRoundsSwitcher BuildRoundsSwitcher(IRoundState[] rounds)
        {
            var roundsSwitcherBuilder = new RoundsSwitcher.Builder(this.stateMachine);

            for (int i = 0; i < rounds.Length; i++)
            {
                var round = rounds[i];
                roundsSwitcherBuilder.AddRound(round);
            }

            return roundsSwitcherBuilder.Build();
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
                ServerNetworkManager.Instance.KickPlayer(connectionId, "Presenter already connected");//TODO: Transate
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
            ServerNetworkManager.Instance.SendClientCommand(this.PresenterId, activateStateCommand);
        }

        private void OnMainPlayerConnecting(int connectionId)
        {
            if (this.stateMachine.CurrentState == this.playersConnectingToTheServerState ||
                this.mainPlayersConnectionIds.Contains(connectionId))
            {
                return;
            }

            ServerNetworkManager.Instance.KickPlayer(connectionId);
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
                ServerNetworkManager.Instance.SendClientCommand(connectionId, command);
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

        public void AddMainPlayerToSurrenderList(int connectionId)
        {
            this.surrenderedMainPlayersConnectionIds.Add(connectionId);
        }

        public void EndGame()
        {
            var endGameState = new EndGameState(ServerNetworkManager.Instance, this.gameDataIterator);
            this.stateMachine.SetCurrentState(endGameState);

            this.IsGameOver = true;
            this.OnGameOver(this, EventArgs.Empty);
        }
    }
}