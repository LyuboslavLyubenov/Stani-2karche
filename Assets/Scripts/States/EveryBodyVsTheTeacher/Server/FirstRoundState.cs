namespace States.EveryBodyVsTheTeacher.Server
{
    using System;

    using Assets.Scripts.Commands.EveryBodyVsTheTeacher;
    using Assets.Scripts.Interfaces.States.EveryBodyVsTheTeacher.Server;
    using Assets.Scripts.Utils.States.EveryBodyVsTheTeacher;

    using Commands;

    using EventArgs;
    
    using Interfaces.Commands.Jokers.Selected;
    using Interfaces.GameData;
    using Interfaces.Network;
    using Interfaces.Network.NetworkManager;

    using Jokers;
    using Jokers.Kalitko;

    using Network;

    using StateMachine;

    using Zenject.Source.Usage;

    public class FirstRoundState : IRoundState
    {
        private const int MaxIncorrectAnswersAllowed = 3;

        private readonly Type[] jokersForThisRound = new []
                                                            {
                                                                typeof(MainPlayerKalitkoJoker),
                                                                typeof(TrustRandomPersonJoker),
                                                                typeof(ConsultWithTheTeacherJoker)
                                                            };

        
        public event EventHandler OnMustGoOnNextRound = delegate {};
        public event EventHandler OnTooManyWrongAnswers = delegate {};

        [Inject]
        private IEveryBodyVsTheTeacherServer server;

        [Inject]
        private JokersData jokersData;

        [Inject]
        private IGameDataExtractor gameDataExtractor;

        [Inject]
        private IGameDataIterator gameDataIterator;

        [Inject]
        private IServerNetworkManager networkManager;
        
        [Inject]
        private ICollectVoteResultForAnswerForCurrentQuestion currentQuestionAnswersCollector;

        [Inject]
        private IElectionJokerCommand[] selectJokerCommands;

        private int incorrectAnswersCount = 0;
        
        private void InitializeSelectJokerCommands()
        {
            var commandsManager = this.networkManager.CommandsManager;
            commandsManager.AddCommands(this.selectJokerCommands);
        }
        
        public void OnStateEnter(StateMachine stateMachine)
        {
            this.gameDataExtractor.ExtractDataSync();

            for (int i = 0; i < this.jokersForThisRound.Length; i++)
            {
                var jokerToAdd = this.jokersForThisRound[i];
                this.jokersData.AddJoker(jokerToAdd);
            }
            
            this.InitializeSelectJokerCommands();
            this.currentQuestionAnswersCollector.StartCollecting();
            this.currentQuestionAnswersCollector.OnCollectedVote += this.OnCollectedVoteForAnswerForCurrentQuestion;
        }
 
        private void OnGetQuestionError(object sender, UnhandledExceptionEventArgs args)
        {
            //TODO:
        }

        private void UseNextQuestion()
        {
           this.gameDataIterator.GetNextQuestion(
               (question) =>
                   {
                       this.currentQuestionAnswersCollector.StartCollecting();   
                   },
               (error) =>
                   {
                       this.OnGetQuestionError(this, new UnhandledExceptionEventArgs(error, false));    
                   });
        }

        private void OnIncorrectAnswer()
        {
            this.incorrectAnswersCount++;

            var incorrectAnswerCommand = NetworkCommandData.From<InCorrectAnswerCommand>();
            this.networkManager.SendClientCommand(this.server.PresenterId, incorrectAnswerCommand);
            
            if (this.incorrectAnswersCount > MaxIncorrectAnswersAllowed)
            {
                this.OnTooManyWrongAnswers(this, EventArgs.Empty);
            }
        }

        private void OnCollectedVoteForAnswerForCurrentQuestion(object sender, AnswerEventArgs args)
        {
            this.gameDataIterator.GetCurrentQuestion(
                (question) =>
                    {
                        if (question.CorrectAnswer == args.Answer)
                        {
                            this.UseNextQuestion();
                        }
                        else
                        {
                            this.OnIncorrectAnswer();
                        }
                    },
                (error) =>
                    {
                        this.OnGetQuestionError(sender, new UnhandledExceptionEventArgs(error, false));
                    });
        }

        public void OnStateExit(StateMachine stateMachine)
        {
            JokersUtils.RemoveRemainingJokers(jokersForThisRound, this.jokersData);
            JokersUtils.RemoveSelectJokerCommands(this.networkManager, this.selectJokerCommands);
        }
    }
}