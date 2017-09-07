using AnswerEventArgs = EventArgs.AnswerEventArgs;
using ElectionJokersActionNotifier = Network.EveryBodyVsTheTeacher.ElectionJokersActionNotifier;
using ICollectVoteResultForAnswerForCurrentQuestion = Interfaces.Network.ICollectVoteResultForAnswerForCurrentQuestion;
using IElectionJokerCommand = Interfaces.Commands.Jokers.Selected.IElectionJokerCommand;
using IEveryBodyVsTheTeacherServer = Interfaces.Network.IEveryBodyVsTheTeacherServer;
using IGameDataIterator = Interfaces.GameData.IGameDataIterator;
using IServerNetworkManager = Interfaces.Network.NetworkManager.IServerNetworkManager;
using JokersData = Network.JokersData;
using MarkEventArgs = EventArgs.MarkEventArgs;
using NetworkCommandData = Commands.NetworkCommandData;

namespace Assets.Scripts.States.EveryBodyVsTheTeacher.Server.Rounds
{
    using System;

    using Assets.Scripts.Commands.EveryBodyVsTheTeacher;
    using Assets.Scripts.Interfaces.States.EveryBodyVsTheTeacher.Server;
    using Assets.Scripts.Utils.States.EveryBodyVsTheTeacher.Server;

    using StateMachine;

    using ArgumentNullException = System.ArgumentNullException;
    using ArgumentOutOfRangeException = System.ArgumentOutOfRangeException;
    using UnhandledExceptionEventArgs = System.UnhandledExceptionEventArgs;

    public abstract class RoundStateAbstract : IRoundState
    {
        public event EventHandler OnMustGoOnNextRound = delegate { };
        public event EventHandler OnMustEndGame = delegate { };
        public event EventHandler OnSelectedInCorrectAnswer = delegate { };
        public event EventHandler<UnhandledExceptionEventArgs> OnLoadQuestionError = delegate { };

        protected readonly IGameDataIterator gameDataIterator;
        protected readonly IServerNetworkManager networkManager;
        protected readonly IEveryBodyVsTheTeacherServer server;
        protected readonly ICollectVoteResultForAnswerForCurrentQuestion currentQuestionAnswersCollector;
        protected readonly JokersData jokersData;

        private readonly Type[] jokersForThisRound;

        private readonly int maxInCorrectAnswersAllowed;
        private int inCorrectAnswersCount = 0;

        private readonly IElectionJokerCommand[] selectedJokerCommands;
        private ElectionJokersActionNotifier electionJokersActionNotifier;

        public int MistakesRemaining
        {
            get
            {
                return this.maxInCorrectAnswersAllowed - this.inCorrectAnswersCount;
            }
        }

        protected RoundStateAbstract(
            IServerNetworkManager networkManager,
            IEveryBodyVsTheTeacherServer server,
            IGameDataIterator gameDataIterator, 
            ICollectVoteResultForAnswerForCurrentQuestion currentQuestionAnswersCollector,
            JokersData jokersData,
            Type[] jokersForThisRound,
            IElectionJokerCommand[] selectedJokerCommands,
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

            if (jokersForThisRound == null)
            {
                throw new ArgumentNullException("jokersForThisRound");
            }

            if (selectedJokerCommands == null)
            {
                throw new ArgumentNullException("selectedJokerCommands");
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
            this.jokersForThisRound = jokersForThisRound;
            this.selectedJokerCommands = selectedJokerCommands;
            this.maxInCorrectAnswersAllowed = maxInCorrectAnswersAllowed;
        }
        
        private void OnIncorrectAnswer()
        {
            this.inCorrectAnswersCount++;

            var incorrectAnswerCommand = NetworkCommandData.From<InCorrectAnswerCommand>();
            this.networkManager.SendClientCommand(this.server.PresenterId, incorrectAnswerCommand);

            if (this.inCorrectAnswersCount > this.maxInCorrectAnswersAllowed)
            {
                this.OnMustEndGame(this, System.EventArgs.Empty);
            }
            else
            {
                this.currentQuestionAnswersCollector.StartCollecting();
                this.OnSelectedInCorrectAnswer(this, EventArgs.Empty);
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
        
        private void OnPlayerVotedForCurrentQuestion(object sender, AnswerEventArgs args)
        {
            var answerSelectedCommand = new NetworkCommandData("AnswerSelected");
            answerSelectedCommand.AddOption("ConnectionId", "0");
            answerSelectedCommand.AddOption("Answer", args.Answer);
            this.networkManager.SendClientCommand(this.server.PresenterId, answerSelectedCommand);
        }

        private void OnNoVotesCollected(object sender, EventArgs args)
        {
            this.OnMustEndGame(this, EventArgs.Empty);
        }

        private void OnMarkIncrease(object sender, MarkEventArgs args)
        {
            this.OnMustGoOnNextRound(this, EventArgs.Empty);
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

        private void AddJokersForThisRound()
        {
            for (int i = 0; i < this.jokersForThisRound.Length; i++)
            {
                var jokerToAdd = this.jokersForThisRound[i];
                this.jokersData.AddJoker(jokerToAdd);
            }
        }

        private void InitializeSelectJokerCommands()
        {
            var commandsManager = this.networkManager.CommandsManager;
            commandsManager.AddCommands(this.selectedJokerCommands);

            this.electionJokersActionNotifier = new ElectionJokersActionNotifier(
                this.networkManager,
                this.server,
                this.selectedJokerCommands);
        }

        public virtual void OnStateEnter(StateMachine stateMachine)
        {
            this.gameDataIterator.OnMarkIncrease += this.OnMarkIncrease;
            this.currentQuestionAnswersCollector.OnNoVotesCollected += this.OnNoVotesCollected;
            this.currentQuestionAnswersCollector.OnCollectedVote += this.OnCollectedVoteForAnswerForCurrentQuestion;
            this.currentQuestionAnswersCollector.OnPlayerVoted += this.OnPlayerVotedForCurrentQuestion;

            this.currentQuestionAnswersCollector.StartCollecting();
            
            this.InitializeSelectJokerCommands();
            this.AddJokersForThisRound();
        }
        
        public virtual void OnStateExit(StateMachine stateMachine)
        {
            JokersUtils.RemoveRemainingJokers(this.jokersForThisRound, this.jokersData);
            JokersUtils.RemoveSelectJokerCommands(this.networkManager, this.selectedJokerCommands);
            this.electionJokersActionNotifier.Dispose();
        }
    }
}