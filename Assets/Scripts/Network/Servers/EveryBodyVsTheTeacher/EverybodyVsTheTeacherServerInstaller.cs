using PlayersConnectingToTheServerState = States.EveryBodyVsTheTeacher.Server.PlayersConnectingToTheServerState;

namespace Network.Servers.EveryBodyVsTheTeacher
{

    using System;

    using Assets.Scripts.Interfaces.Network;
    using Assets.Scripts.Interfaces.States.EveryBodyVsTheTeacher.Server;
    using Assets.Scripts.Network;
    using Assets.Scripts.Network.EveryBodyVsTheTeacher;
    using Assets.Scripts.Network.JokersData.EveryBodyVsTheTeacher;
    using Assets.Scripts.States.EveryBodyVsTheTeacher.Server.Rounds;

    using Interfaces;
    using Interfaces.GameData;
    using Interfaces.Network;
    using Interfaces.Network.EveryBodyVsTheTeacher;
    using Interfaces.Network.EveryBodyVsTheTeacher.States;
    using Interfaces.Network.NetworkManager;

    using IO;

    using Network.EveryBodyVsTheTeacher.PlayersConnectingState;
    using Network.GameInfo;
    using Network.NetworkManagers;
    using Network.TcpSockets;

    using StateMachine;

    using UnityEngine;

    using Zenject.Source.Install;

    public class EverybodyVsTheTeacherServerInstaller : MonoInstaller
    {
        private const int ServerPort = 7772;

        [SerializeField]
        private EveryBodyVsTheTeacherServer Server;
        
        private void InstallServerNetworkManager()
        {
            var serverNetworkManager = ServerNetworkManager.Instance;

            this.Container.Bind<IServerNetworkManager>()
                .To<ServerNetworkManager>()
                .FromInstance(serverNetworkManager)
                .AsSingle();
        }

        private void InstallServer()
        {
            this.Container.Bind<IEveryBodyVsTheTeacherServer>()
                .FromInstance(Server)
                .AsSingle();
        }

        private void InstallGameDataExtractor()
        {
            this.Container.Bind<IGameDataExtractor>()
                .To<GameDataExtractor>()
                .AsSingle();
        }

        private void InstallGameDataIterator()
        {
            this.Container.Bind<IGameDataIterator>()
                .To<GameDataIterator>()
                .AsSingle();
        }

        private void InstallAnswersCollector(
            IEveryBodyVsTheTeacherServer server, 
            IServerNetworkManager networkManager, 
            IGameDataIterator gameDataIterator)
        {
            var answersCollector = new VoteResultForAnswerForCurrentQuestionCollector(server, networkManager, gameDataIterator);

            this.Container.Bind<ICollectVoteResultForAnswerForCurrentQuestion>()
                .FromInstance(answersCollector)
                .AsSingle();
        }
        
        private void InstallStateMachine()
        {
            this.Container.Bind<StateMachine>()
                .AsSingle();
        }

        private void InstallCreatedGameInfoSender(IServerNetworkManager networkManager, IGameServer gameServer)
        {
            var tcpClient = new SimpleTcpClient();
            var tcpServer = new SimpleTcpServer(ServerPort);
            var gameInfoFactory = GameInfoFactory.Instance;

            var createdGameInfoSender =
                new CreatedGameInfoSender(
                    tcpClient,
                    tcpServer,
                    gameInfoFactory,
                    networkManager,
                    gameServer);

            this.Container.Bind<ICreatedGameInfoSender>()
                .FromInstance(createdGameInfoSender)
                .AsSingle();
        }

        private void InstallFirstRound(
            IServerNetworkManager networkManager, 
            IEveryBodyVsTheTeacherServer server, 
            IGameDataIterator gameDataIterator, 
            IGameDataExtractor gameDataExtractor, 
            ICollectVoteResultForAnswerForCurrentQuestion currentQuestionAnswersCollector, 
            JokersData jokersData)
        {
            var firstRoundBuilder = new FirstRoundState.Builder
                                    {
                                        GameDataExtractor = gameDataExtractor,
                                        Server = server,
                                        CurrentQuestionAnswersCollector = currentQuestionAnswersCollector,
                                        GameDataIterator = gameDataIterator,
                                        JokersData = jokersData,
                                        ServerNetworkManager = networkManager
                                    };
            var firstRound = firstRoundBuilder.Build();

            this.Container.Bind<FirstRoundState>()
                .FromInstance(firstRound)
                .AsSingle();
        }

        public void InstallSecondRound(
            IServerNetworkManager networkManager,
            IEveryBodyVsTheTeacherServer server,
            IGameDataIterator gameDataIterator,
            ICollectVoteResultForAnswerForCurrentQuestion currentQuestionAnswersCollector,
            JokersData jokersData)
        {
            var secondRoundBuilder = new SecondRoundState.Builder()
                                     {
                                         CurrentQuestionAnswersCollector = currentQuestionAnswersCollector,
                                         GameDataIterator = gameDataIterator,
                                         JokersData = jokersData,
                                         Server = server,
                                         ServerNetworkManager = networkManager
                                     };
            var secondRound = secondRoundBuilder.Build();
            this.Container.Bind<SecondRoundState>()
                .FromInstance(secondRound)
                .AsSingle();
        }

        private void InstallThirdRound(
            IServerNetworkManager networkManager,
            IEveryBodyVsTheTeacherServer server,
            IGameDataIterator gameDataIterator,
            ICollectVoteResultForAnswerForCurrentQuestion currentQuestionAnswersCollector,
            JokersData jokersData)
        {
            var thirdRoundBuilder = new ThirdRoundState.Builder()
                                    {
                                        CurrentQuestionAnswersCollector = currentQuestionAnswersCollector,
                                        GameDataIterator = gameDataIterator,
                                        JokersData = jokersData,
                                        Server = server,
                                        ServerNetworkManager = networkManager
                                    };
            var thirdRound = thirdRoundBuilder.Build();
            this.Container.Bind<ThirdRoundState>()
                .FromInstance(thirdRound)
                .AsSingle();
        }

        private void InstallRoundsSwitcher(StateMachine stateMachine, IRoundState[] rounds)
        {
            var roundsSwitcherBuilder = new RoundsSwitcher.Builder(stateMachine);

            for (int i = 0; i < rounds.Length; i++)
            {
                var round = rounds[i];
                roundsSwitcherBuilder.AddRound(round);
            }

            var roundsSwitcher = roundsSwitcherBuilder.Build();

            roundsSwitcher.OnMustEndGame += OnMustEndGame;

            this.Container.Bind<IRoundsSwitcher>()
                .FromInstance(roundsSwitcher)
                .AsSingle();
        }

        private void OnMustEndGame(object sender, EventArgs args)
        {
            var server = this.Container.Resolve<IEveryBodyVsTheTeacherServer>();
            server.EndGame();
        }

        private void InstallRoundsSwitcherEventsNotifier()
        {
            this.Container.Bind<RoundsSwitcherEventsNotifier>()
                .ToSelf()
                .AsSingle();
        }

        public override void InstallBindings()
        {
            this.InstallServerNetworkManager();
            this.InstallServer();
            this.InstallGameDataExtractor();
            this.InstallGameDataIterator();
            
            this.Container.Bind<JokersData>()
                .AsSingle();
            
            this.Container.Bind<IPlayersConnectingToTheServerState>()
                .To<PlayersConnectingToTheServerState>()
                .AsSingle();

            this.Container.Bind<int>()
                .FromInstance(6)
                .WhenInjectedInto<IPlayersConnectingStateDataSender>();

            this.Container.Bind<IPlayersConnectingStateDataSender>()
                .To<PlayersConnectingStateDataSender>();

            this.Container.Bind<ISecondsRemainingUICommandsSender>()
                .To<SecondsRemainingUICommandsSender>()
                .AsSingle();

            this.Container.Bind<IQuestionsRemainingCommandsSender>()
                .To<QuestionsRemainingUICommandsSender>()
                .AsSingle();

            this.Container.Bind<IMistakesRemainingCommandsSender>()
                .To<MistakesRemainingCommandsSender>()
                .AsSingle();

            this.Container.Bind<JokersDataSender>()
                .ToSelf()
                .AsSingle();

            this.InstallStateMachine();

            var networkManager = this.Container.Resolve<IServerNetworkManager>();
            var iterator = this.Container.Resolve<IGameDataIterator>();

            this.InstallAnswersCollector(this.Server, networkManager, iterator);
            this.InstallCreatedGameInfoSender(networkManager, this.Server);

            var extractor = this.Container.Resolve<IGameDataExtractor>();
            var answersCollector = this.Container.Resolve<ICollectVoteResultForAnswerForCurrentQuestion>();
            var jokersData = this.Container.Resolve<JokersData>();

            this.InstallFirstRound(networkManager, this.Server, iterator, extractor, answersCollector, jokersData);
            this.InstallSecondRound(networkManager, this.Server, iterator, answersCollector, jokersData);
            this.InstallThirdRound(networkManager, this.Server, iterator, answersCollector, jokersData);

            var stateMachine = this.Container.Resolve<StateMachine>();
            var rounds = new IRoundState[]
                         {
                             this.Container.Resolve<FirstRoundState>(),
                             this.Container.Resolve<SecondRoundState>(),
                             this.Container.Resolve<ThirdRoundState>()
                         };

            this.InstallRoundsSwitcher(stateMachine, rounds);
            this.InstallRoundsSwitcherEventsNotifier();
        }
    }
}