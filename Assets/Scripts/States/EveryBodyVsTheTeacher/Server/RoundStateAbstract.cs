using ICollectVoteResultForAnswerForCurrentQuestion = Interfaces.Network.ICollectVoteResultForAnswerForCurrentQuestion;
using IEveryBodyVsTheTeacherServer = Interfaces.Network.IEveryBodyVsTheTeacherServer;
using IGameDataIterator = Interfaces.GameData.IGameDataIterator;
using IServerNetworkManager = Interfaces.Network.NetworkManager.IServerNetworkManager;
using JokersData = Network.JokersData;
using NetworkCommandData = Commands.NetworkCommandData;

namespace Assets.Scripts.States.EveryBodyVsTheTeacher.Server
{

    using System;

    using Assets.Scripts.Commands.EveryBodyVsTheTeacher;
    using Assets.Scripts.Interfaces;

    using EventArgs;

    using StateMachine;

    using ArgumentNullException = System.ArgumentNullException;
    using ArgumentOutOfRangeException = System.ArgumentOutOfRangeException;
    using UnhandledExceptionEventArgs = System.UnhandledExceptionEventArgs;

    public abstract class RoundStateAbstract : IState
    {
        public event EventHandler OnMustGoOnNextRound = delegate { };
        public event EventHandler OnTooManyWrongAnswers = delegate { };
        public event EventHandler<UnhandledExceptionEventArgs> OnLoadQuestionError = delegate { };

        protected readonly IGameDataIterator gameDataIterator;
        protected readonly IServerNetworkManager networkManager;
        protected readonly IEveryBodyVsTheTeacherServer server;
        protected readonly ICollectVoteResultForAnswerForCurrentQuestion currentQuestionAnswersCollector;
        protected readonly JokersData jokersData;

        private readonly int maxInCorrectAnswersAllowed;

        private int inCorrectAnswersCount = 0;

        protected RoundStateAbstract(
            IServerNetworkManager networkManager,
            IEveryBodyVsTheTeacherServer server,
            IGameDataIterator gameDataIterator, 
            ICollectVoteResultForAnswerForCurrentQuestion currentQuestionAnswersCollector,
            JokersData jokersData,
            int maxInCorrectAnswersAllowed)
        {
            if (networkManager == null)
            {
                throw new ArgumentNullException("networkManager");
            }

            if (server == null)
            {
                throw new ArgumentNullException("server");
            }

            if (gameDataIterator == null)
            {
                throw new ArgumentNullException("gameDataIterator");
            }

            if (currentQuestionAnswersCollector == null)
            {
                throw new ArgumentNullException("currentQuestionAnswersCollector");
            }

            if (jokersData == null)
            {
                throw new ArgumentNullException("jokersData");
            }

            if (maxInCorrectAnswersAllowed < 0)
            {
                throw new ArgumentOutOfRangeException("maxInCorrectAnswersAllowed");
            }

            this.networkManager = networkManager;
            this.server = server;
            this.gameDataIterator = gameDataIterator;
            this.currentQuestionAnswersCollector = currentQuestionAnswersCollector;
            this.jokersData = jokersData;
            this.maxInCorrectAnswersAllowed = maxInCorrectAnswersAllowed;
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
                        this.OnLoadQuestionError(this, new UnhandledExceptionEventArgs(error, false));
                    });
        }

        private void OnIncorrectAnswer()
        {
            this.inCorrectAnswersCount++;

            var incorrectAnswerCommand = NetworkCommandData.From<InCorrectAnswerCommand>();
            this.networkManager.SendClientCommand(this.server.PresenterId, incorrectAnswerCommand);

            if (this.inCorrectAnswersCount > this.maxInCorrectAnswersAllowed)
            {
                this.OnTooManyWrongAnswers(this, System.EventArgs.Empty);
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
                        this.OnLoadQuestionError(sender, new UnhandledExceptionEventArgs(error, false));
                    });
        }

        private void OnMarkIncrease(object sender, MarkEventArgs args)
        {
            this.OnMustGoOnNextRound(this, EventArgs.Empty);
        }

        protected void AddJokersForThisRound(Type[] jokersTypes)
        {
            for (int i = 0; i < jokersTypes.Length; i++)
            {
                var jokerToAdd = jokersTypes[i];
                this.jokersData.AddJoker(jokerToAdd);
            }
        }

        public virtual void OnStateEnter(StateMachine stateMachine)
        {
            this.currentQuestionAnswersCollector.StartCollecting();
            this.currentQuestionAnswersCollector.OnCollectedVote += this.OnCollectedVoteForAnswerForCurrentQuestion;
            this.gameDataIterator.OnMarkIncrease += OnMarkIncrease;
        }


        public abstract void OnStateExit(StateMachine stateMachine);
    }
}
