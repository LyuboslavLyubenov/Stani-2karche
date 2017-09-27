using HelpFromFriendJoker = Jokers.HelpFromFriendJoker;
using ICollectVoteResultForAnswerForCurrentQuestion = Interfaces.Network.ICollectVoteResultForAnswerForCurrentQuestion;
using IElectionJokerCommand = Interfaces.Commands.Jokers.Selected.IElectionJokerCommand;
using IEveryBodyVsTheTeacherServer = Interfaces.Network.IEveryBodyVsTheTeacherServer;
using IGameDataIterator = Interfaces.GameData.IGameDataIterator;
using IServerNetworkManager = Interfaces.Network.NetworkManager.IServerNetworkManager;
using JokersData = Network.JokersData;
using SelectedTrustRandomPersonJokerCommand = Commands.Jokers.Selected.SelectedTrustRandomPersonJokerCommand;
using TrustRandomPersonJokerRouter = Jokers.Routers.TrustRandomPersonJokerRouter;

namespace Assets.Scripts.States.EveryBodyVsTheTeacher.Server.Rounds
{

    using System;

    public class ThirdRoundState : RoundStateAbstract
    {
        private const int MaxIncorrectAnswersAllowed = 2;

        private static readonly Type[] jokersForThisRound = new Type[]
                                                     {
                                                         typeof(HelpFromFriendJoker)
                                                     };

        public class Builder : RoundBuilder
        {
            public ThirdRoundState Build()
            {
                var trustRanomPersonJokerRouter = 
                    new TrustRandomPersonJokerRouter(base.ServerNetworkManager, base.Server, base.GameDataIterator);
                var selectedTrustRandomPersonJokerCommand =
                    new SelectedTrustRandomPersonJokerCommand(base.Server, this.JokersData, trustRanomPersonJokerRouter);
                var commands = new IElectionJokerCommand[]
                               {
                                   selectedTrustRandomPersonJokerCommand
                               };
                return 
                    new ThirdRoundState(
                        base.ServerNetworkManager, 
                        base.Server,
                        base.GameDataIterator,
                        base.CurrentQuestionAnswersCollector,
                        base.JokersData,
                        commands);
            }
        }
        
        public ThirdRoundState(
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
                  jokersForThisRound, 
                  selectedJokerCommands, 
                  MaxIncorrectAnswersAllowed)
        {
        }
    }
}