using ConsultWithTheTeacherJoker = Jokers.ConsultWithTheTeacherJoker;
using DisableRandomAnswersJokerRouter = Jokers.Routers.DisableRandomAnswersJokerRouter;
using ICollectVoteResultForAnswerForCurrentQuestion = Interfaces.Network.ICollectVoteResultForAnswerForCurrentQuestion;
using IElectionJokerCommand = Interfaces.Commands.Jokers.Selected.IElectionJokerCommand;
using IEveryBodyVsTheTeacherServer = Interfaces.Network.IEveryBodyVsTheTeacherServer;
using IGameDataExtractor = Interfaces.GameData.IGameDataExtractor;
using IGameDataIterator = Interfaces.GameData.IGameDataIterator;
using IServerNetworkManager = Interfaces.Network.NetworkManager.IServerNetworkManager;
using JokersData = Network.JokersData;
using KalitkoJokerRouter = Jokers.Routers.KalitkoJokerRouter;
using MainPlayerKalitkoJoker = Jokers.Kalitko.MainPlayerKalitkoJoker;
using PlayerPrefsEncryptionUtils = Utils.Unity.PlayerPrefsEncryptionUtils;
using SelectedConsultWithTeacherJokerCommand = Commands.Jokers.Selected.SelectedConsultWithTeacherJokerCommand;
using SelectedKalitkoJokerCommand = Commands.Jokers.Selected.SelectedKalitkoJokerCommand;
using SelectedTrustRandomPersonJokerCommand = Commands.Jokers.Selected.SelectedTrustRandomPersonJokerCommand;
using TrustRandomPersonJoker = Jokers.TrustRandomPersonJoker;
using TrustRandomPersonJokerRouter = Jokers.Routers.TrustRandomPersonJokerRouter;

namespace Assets.Scripts.States.EveryBodyVsTheTeacher.Server.Rounds
{

    using System;

    using StateMachine;

    public class FirstRoundState : RoundStateAbstract
    {
        private const int MaxIncorrectAnswersAllowed = 4;

        private static readonly Type[] JokersForThisRound = new[]
                                                            {
                                                                typeof(MainPlayerKalitkoJoker),
                                                                typeof(TrustRandomPersonJoker),
                                                                typeof(ConsultWithTheTeacherJoker)
                                                            };

        private readonly IGameDataExtractor gameDataExtractor;

        public class Builder : RoundBuilder
        {

            public IGameDataExtractor GameDataExtractor
            {
                get;
                set;
            }

            public FirstRoundState Build()
            {
                var kalitkoJokerRouter = new KalitkoJokerRouter(base.ServerNetworkManager, base.Server, base.GameDataIterator);
                var trustRandomPersonJokerRouter = new TrustRandomPersonJokerRouter(base.ServerNetworkManager, base.Server, base.GameDataIterator);
                var disableRandomAnswersJokerRouter = new DisableRandomAnswersJokerRouter(base.ServerNetworkManager);

                var selectedKalitkoJokerCommand = new SelectedKalitkoJokerCommand(base.Server, kalitkoJokerRouter);
                var selectedTrustRandomPersonJokerCommand = new SelectedTrustRandomPersonJokerCommand(base.Server, trustRandomPersonJokerRouter);
                var selectedConsultWithTeacherJokerCommand = new SelectedConsultWithTeacherJokerCommand(base.Server, disableRandomAnswersJokerRouter);

                var commands = new IElectionJokerCommand[]
                               {
                                   selectedKalitkoJokerCommand,
                                   selectedTrustRandomPersonJokerCommand,
                                   selectedConsultWithTeacherJokerCommand
                               };

                return
                    new FirstRoundState(
                        base.ServerNetworkManager,
                        base.Server,
                        base.GameDataIterator,
                        this.GameDataExtractor,
                        base.CurrentQuestionAnswersCollector,
                        base.JokersData,
                        commands);
            }
        }

        private FirstRoundState(
            IServerNetworkManager networkManager,
            IEveryBodyVsTheTeacherServer server,
            IGameDataIterator gameDataIterator,
            IGameDataExtractor gameDataExtractor,
            ICollectVoteResultForAnswerForCurrentQuestion currentQuestionAnswersCollector,
            JokersData jokersData,
            IElectionJokerCommand[] electionJokersCommands)
            : base(
                  networkManager,
                  server,
                  gameDataIterator,
                  currentQuestionAnswersCollector,
                  jokersData,
                  JokersForThisRound,
                  electionJokersCommands,
                  MaxIncorrectAnswersAllowed)
        {
            if (gameDataExtractor == null)
            {
                throw new ArgumentNullException("gameDataExtractor");
            }

            this.gameDataExtractor = gameDataExtractor;
        }

        public override void OnStateEnter(StateMachine stateMachine)
        {
#if UNITY_EDITOR
            if (!PlayerPrefsEncryptionUtils.HasKey("LevelCategory"))
            {
                PlayerPrefsEncryptionUtils.SetString("LevelCategory", "философия");
            }
#endif

            this.gameDataExtractor.LevelCategory = PlayerPrefsEncryptionUtils.GetString("LevelCategory");
            this.gameDataExtractor.ExtractDataSync();

            base.OnStateEnter(stateMachine);
        }
    }
}