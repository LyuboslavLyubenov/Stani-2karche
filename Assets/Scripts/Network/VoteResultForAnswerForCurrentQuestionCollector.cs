using IGameDataIterator = Interfaces.GameData.IGameDataIterator;
using IServerNetworkManager = Interfaces.Network.NetworkManager.IServerNetworkManager;
using ISimpleQuestion = Interfaces.ISimpleQuestion;
using LoadQuestionCommand = Commands.Client.LoadQuestionCommand;
using NetworkCommandData = Commands.NetworkCommandData;
using SelectedAnswerCommand = Commands.Server.SelectedAnswerCommand;

namespace Network
{

    using System;
    using System.Collections.Generic;
    using System.Linq;

    using EventArgs;

    using Interfaces.Network;

    using UnityEngine;

    using Utils;

    public class VoteResultForAnswerForCurrentQuestionCollector : ICollectVoteResultForAnswerForCurrentQuestion
    {
        public event EventHandler<AnswerEventArgs> OnCollectedVote = delegate { };
        public event EventHandler OnNoVotesCollected = delegate { };
        public event EventHandler<UnhandledExceptionEventArgs> OnLoadingCurrentQuestionError = delegate {};

        private IEveryBodyVsTheTeacherServer server;
        private IServerNetworkManager networkManager;
        private IGameDataIterator gameDataIterator;

        private SelectedAnswerCommand answerSelectedCommand;

        private List<int> clientsVoted = new List<int>();
        private Dictionary<string, int> answersVotesCount = new Dictionary<string, int>();

        private string[] possibleAnswers;

        private Timer_ExecuteMethodAfterTime voteTimeoutTimer;

        public bool Collecting
        {
            get;
            private set;
        }

        /// <summary>
        /// Collects answers from main players for current question. When collected all answers or time was over -> raise OnCollectedVote() with highest voted answer
        /// </summary>
        public VoteResultForAnswerForCurrentQuestionCollector(
            IEveryBodyVsTheTeacherServer server,
            IServerNetworkManager networkManager,
            IGameDataIterator gameDataIterator)
        {
            this.server = server;
            this.networkManager = networkManager;
            this.gameDataIterator = gameDataIterator;

            this.answerSelectedCommand = new SelectedAnswerCommand(this.OnReceivedAnswer);
        }

        private void ConfigureTimer(int timeToAnswerInSeconds)
        {
            this.voteTimeoutTimer = TimerUtils.ExecuteAfter(timeToAnswerInSeconds, this.OnReceivedAllAnswersTimeout);
            this.voteTimeoutTimer.RunOnUnityThread = true;
            this.voteTimeoutTimer.AutoDispose = true;
            this.voteTimeoutTimer.Stop();
        }

        private void OnReceivedAnswer(int connectionId, string answer)
        {
            if (!this.server.MainPlayersConnectionIds.Contains(connectionId) ||
                this.clientsVoted.Contains(connectionId) ||
                !this.possibleAnswers.Contains(answer))
            {
                return;
            }

            this.clientsVoted.Add(connectionId);

            if (!this.answersVotesCount.ContainsKey(answer))
            {
                this.answersVotesCount.Add(answer, 0);
            }

            this.answersVotesCount[answer]++;

            if (this.clientsVoted.Count != this.server.MainPlayersConnectionIds.Count())
            {
                return;
            }

            this.StopCollecting();
            this.RaiseOnCollectedVoteEvent();
        }

        private void OnReceivedAllAnswersTimeout()
        {
            this.StopCollecting();
            this.RaiseOnCollectedVoteEvent();
        }

        private void RaiseOnCollectedVoteEvent()
        {
            if (this.answersVotesCount.Count == 0)
            {
                this.OnNoVotesCollected(this, EventArgs.Empty);
            }
            else
            {
                var highestVotedAnswer = this.answersVotesCount.OrderByDescending(answerVotesCount => answerVotesCount.Value)
                .First()
                .Key;
                this.OnCollectedVote(this, new AnswerEventArgs(highestVotedAnswer, null));
            }
        }

        private void SendQuestionToMainPlayers(ISimpleQuestion question, int timeToAnswerInSeconds)
        {
            var loadQuestionCommand = NetworkCommandData.From<LoadQuestionCommand>();
            var questionJSON = JsonUtility.ToJson(question.Serialize());

            loadQuestionCommand.AddOption("QuestionJSON", questionJSON);
            loadQuestionCommand.AddOption("TimeToAnswer", timeToAnswerInSeconds.ToString());
            
            this.server.MainPlayersConnectionIds.ToList()
                .ForEach(connectionId => this.networkManager.SendClientCommand(connectionId, loadQuestionCommand));
        }

        public void StartCollecting()
        {
            if (this.Collecting)
            {
                throw new InvalidOperationException("Still collecting");
            }

            if (!this.gameDataIterator.Loaded)
            {
                throw new InvalidOperationException("GameDataIterator not loaded");
            }

            this.gameDataIterator.GetCurrentQuestion(
                (question) =>
                    {
                        this.clientsVoted.Clear();
                        this.answersVotesCount.Clear();

                        this.possibleAnswers = question.Answers;

                        this.ConfigureTimer(this.gameDataIterator.SecondsForAnswerQuestion);
                        this.SendQuestionToMainPlayers(question, this.gameDataIterator.SecondsForAnswerQuestion);
                        this.voteTimeoutTimer.Start();
                        this.networkManager.CommandsManager.AddCommand("AnswerSelected", this.answerSelectedCommand);

                        this.Collecting = true;
                    },
                (error) =>
                    {
                        this.OnLoadingCurrentQuestionError(this, new UnhandledExceptionEventArgs(error, true));
                    });
        }

        public void StopCollecting()
        {
            if (!this.Collecting)
            {
                throw new InvalidOperationException();
            }
            
            this.voteTimeoutTimer.Stop();
            this.voteTimeoutTimer.Dispose();

            if (this.networkManager.CommandsManager.Exists("AnswerSelected"))
            {
                this.networkManager.CommandsManager.RemoveCommand("AnswerSelected");
            }

            this.Collecting = false;
        }

        public void Dispose()
        {
            if (this.Collecting)
            {
                this.StopCollecting();
            }

            this.OnCollectedVote = null;

            this.voteTimeoutTimer.Stop();
            this.voteTimeoutTimer.Dispose();
            this.voteTimeoutTimer = null;

            this.server = null;
            this.networkManager = null;
            this.gameDataIterator = null;
        }
    }
}