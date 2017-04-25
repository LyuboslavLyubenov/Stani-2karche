using AnswerPollRouter = Jokers.Routers.AnswerPollRouter;
using AskAudienceJoker = Jokers.AskAudienceJoker;
using DisableRandomAnswersJokerRouter = Jokers.Routers.DisableRandomAnswersJokerRouter;
using HelpFromAudienceJokerRouter = Jokers.Routers.HelpFromAudienceJokerRouter;
using ICollectVoteResultForAnswerForCurrentQuestion = Interfaces.Network.ICollectVoteResultForAnswerForCurrentQuestion;
using IElectionJokerCommand = Interfaces.Commands.Jokers.Selected.IElectionJokerCommand;
using IEveryBodyVsTheTeacherServer = Interfaces.Network.IEveryBodyVsTheTeacherServer;
using IGameDataIterator = Interfaces.GameData.IGameDataIterator;
using IServerNetworkManager = Interfaces.Network.NetworkManager.IServerNetworkManager;
using JokersData = Network.JokersData;
using SelectedLittleIsBetterThanNothingJokerCommand = Commands.Jokers.Selected.SelectedLittleIsBetterThanNothingJokerCommand;

namespace Assets.Scripts.States.EveryBodyVsTheTeacher.Server.Rounds
{

    using System;

    using Assets.Scripts.Commands.Jokers.Selected;
    using Assets.Scripts.Jokers;

    public class SecondRoundState : RoundStateAbstract
    {
        private const int MaxInCorrectAnswersAllowed = 3;

        private static readonly Type[] JokersForThisRound = new Type[]
                                                     {
                                                         typeof(LittleIsBetterThanNothingJoker),
                                                         typeof(AskAudienceJoker)
                                                     };

        public class Builder : RoundBuilder
        {
            public SecondRoundState Build()
            {
                var disableRandomAnswersJokerRouter = new DisableRandomAnswersJokerRouter(base.ServerNetworkManager);
                var answerPollRouter = new AnswerPollRouter(base.ServerNetworkManager);
                var helpFromAudienceJokerRouter = new HelpFromAudienceJokerRouter(base.ServerNetworkManager, base.GameDataIterator, answerPollRouter);

                var selectedLittleIsBetterThatNothingJokerCommand =
                    new SelectedLittleIsBetterThanNothingJokerCommand(base.Server, disableRandomAnswersJokerRouter);
                var selectedHelpFromAudienceElectionJokerCommand =
                    new SelectedHelpFromAudienceElectionJokerCommand(base.Server, helpFromAudienceJokerRouter, base.GameDataIterator);
                var commands = new IElectionJokerCommand[]
                               {
                                   selectedLittleIsBetterThatNothingJokerCommand,
                                   selectedHelpFromAudienceElectionJokerCommand
                               };

                return 
                    new SecondRoundState(
                        base.ServerNetworkManager,
                        base.Server,
                        base.GameDataIterator,
                        base.CurrentQuestionAnswersCollector,
                        base.JokersData,
                        commands);
            }
        }

        private SecondRoundState(
            IServerNetworkManager networkManager, 
            IEveryBodyVsTheTeacherServer server, 
            IGameDataIterator gameDataIterator, 
            ICollectVoteResultForAnswerForCurrentQuestion currentQuestionAnswersCollector, 
            JokersData jokersData, 
            IElectionJokerCommand[] selectedJokerCommands)
            : base(
                  networkManager, 
                  server, 
                  gameDataIterator, 
                  currentQuestionAnswersCollector, 
                  jokersData,
                  JokersForThisRound,
                  selectedJokerCommands,
                  MaxInCorrectAnswersAllowed)
        {
        }        
    }
}