namespace States.EveryBodyVsTheTeacher.Server
{
    using System;
    
    using Assets.Scripts.States.EveryBodyVsTheTeacher.Server;

    using Interfaces.Commands.Jokers.Selected;
    using Interfaces.GameData;
    using Interfaces.Network;
    using Interfaces.Network.NetworkManager;

    using Jokers;
    using Jokers.Kalitko;

    using Network;

    using StateMachine;

    public class FirstRoundState : RoundStateAbstract
    {
        private const int MaxIncorrectAnswersAllowed = 3;

        private static readonly Type[] jokersForThisRound = new []
                                                            {
                                                                typeof(MainPlayerKalitkoJoker),
                                                                typeof(TrustRandomPersonJoker),
                                                                typeof(ConsultWithTheTeacherJoker)
                                                            };
        
       
        private readonly IGameDataExtractor gameDataExtractor;
        
        public FirstRoundState(
            IServerNetworkManager networkManager, 
            IEveryBodyVsTheTeacherServer server, 
            IGameDataIterator gameDataIterator, 
            IGameDataExtractor gameDataExtractor,
            ICollectVoteResultForAnswerForCurrentQuestion currentQuestionAnswersCollector, 
            JokersData jokersData,
            IElectionJokerCommand[] electionJokerCommands)
            : base(
                  networkManager, 
                  server, 
                  gameDataIterator, 
                  currentQuestionAnswersCollector, 
                  jokersData,
                  jokersForThisRound,
                  electionJokerCommands,
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
            if (!this.gameDataExtractor.Loaded)
            {
                this.gameDataExtractor.ExtractDataSync();
            }
            
            base.OnStateEnter(stateMachine);
        }
    }
}