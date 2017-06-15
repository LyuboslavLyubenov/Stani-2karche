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

    using Assets.Scripts.Commands.EveryBodyVsTheTeacher;
    using Assets.Scripts.Commands.UI;

    using Commands.Server;

    using EventArgs;

    using Interfaces.Network;

    using UnityEngine;

    using Utils;

    using Zenject.Source.Usage;

    public class VoteResultForAnswerForCurrentQuestionCollector : ICollectVoteResultForAnswerForCurrentQuestion
    {
        public event EventHandler<AnswerEventArgs> OnPlayerVoted = delegate { };
        public event EventHandler<AnswerEventArgs> OnCollectedVote = delegate { };
        public event EventHandler OnNoVotesCollected = delegate { };
        public event EventHandler<UnhandledExceptionEventArgs> OnLoadingCurrentQuestionError = delegate {};

        private readonly IEveryBodyVsTheTeacherServer server;
        private readonly IServerNetworkManager networkManager;
        private readonly IGameDataIterator gameDataIterator;

        private readonly SelectedAnswerCommand answerSelectedCommand;
        private readonly MainPlayerConnectingCommand mainPlayerConnecting;
        private readonly PresenterConnectingCommand presenterConnecting;

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
        [Inject]
        public VoteResultForAnswerForCurrentQuestionCollector(
            IEveryBodyVsTheTeacherServer server,
            IServerNetworkManager networkManager,
            IGameDataIterator gameDataIterator)
        {
            this.server = server;
            this.networkManager = networkManager;
            this.gameDataIterator = gameDataIterator;

            this.networkManager.OnClientDisconnected += OnClientDisconnected;

            this.answerSelectedCommand = new SelectedAnswerCommand(this.OnReceivedAnswer);
            this.mainPlayerConnecting = new MainPlayerConnectingCommand(this.OnMainPlayerConnected);
            this.presenterConnecting = new PresenterConnectingCommand(this.OnPresenterConnecting);
        }

        private void OnClientDisconnected(object sender, ClientConnectionIdEventArgs args)
        {
            if (this.Collecting && !this.server.ConnectedMainPlayersConnectionIds.Any())
            {
                this.voteTimeoutTimer.Pause();
            }
        }

        private void OnMainPlayerConnected(int connectionId)
        {
            if (this.voteTimeoutTimer.Paused)
            {
                this.voteTimeoutTimer.Resume();
            }

            this.SendCurrentQuestionTo(connectionId);
        }

        private void OnPresenterConnecting(int connectionId)
        {
            this.SendCurrentQuestionTo(connectionId);  
            this.ResendVotes(connectionId); 
        }

        private void ResendVotes(int connectionId)
        {
            var votes = this.answersVotesCount.ToArray();
            for (int i = 0; i < votes.Length; i++)
            {
                var vote = votes[i];
                var answerSelectedCommand = new NetworkCommandData("AnswerSelected");
                answerSelectedCommand.AddOption("ConnectionId", "0");
                answerSelectedCommand.AddOption("Answer", vote.Key);

                for (int j = 0; j < vote.Value; j++)
                {
                    this.networkManager.SendClientCommand(connectionId, answerSelectedCommand);
                }
            }
        }
        
        private void SendCurrentQuestionTo(int connectionId)
        {
            this.gameDataIterator.GetCurrentQuestion(
                (question) =>
                    {
                        var timeToAnswerLeft = (int)this.voteTimeoutTimer.RemainingTimeInMiliseconds / 1000;
                        var loadQuestionCommand = this.ConfigureLoadQuestionCommand(question, timeToAnswerLeft);
                        this.networkManager.SendClientCommand(connectionId, loadQuestionCommand);
                    });
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
            if (!this.server.ConnectedMainPlayersConnectionIds.Contains(connectionId) ||
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

            this.OnPlayerVoted(this, new AnswerEventArgs(answer, null));

            if (this.clientsVoted.Count != this.server.ConnectedMainPlayersConnectionIds.Count())
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
                var highestVotedAnswer = this.answersVotesCount
                    .OrderByDescending(answerVotesCount => answerVotesCount.Value)
                    .First()
                    .Key;
                this.OnCollectedVote(this, new AnswerEventArgs(highestVotedAnswer, null));
            }
        }

        private void SendQuestionToMainPlayersAndPresenter(ISimpleQuestion question, int timeToAnswerInSeconds)
        {
            var loadQuestionCommand = this.ConfigureLoadQuestionCommand(question, timeToAnswerInSeconds);
            var connectionIds = this.server.ConnectedMainPlayersConnectionIds.ToList();
            connectionIds.Add(this.server.PresenterId);

            for (int i = 0; i < connectionIds.Count; i++)
            {
                var mainPlayerConnectionId = connectionIds[i];
                this.networkManager.SendClientCommand(mainPlayerConnectionId, loadQuestionCommand);
            }
        }

        private NetworkCommandData ConfigureLoadQuestionCommand(ISimpleQuestion question, int timeToAnswerInSeconds)
        {
            var loadQuestionCommand = NetworkCommandData.From<LoadQuestionCommand>();
            var questionJSON = JsonUtility.ToJson(question.Serialize());

            loadQuestionCommand.AddOption("QuestionJSON", questionJSON);
            loadQuestionCommand.AddOption("TimeToAnswer", timeToAnswerInSeconds.ToString());

            return loadQuestionCommand;
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
                        this.SendQuestionToMainPlayersAndPresenter(question, this.gameDataIterator.SecondsForAnswerQuestion);
                        
                        this.networkManager.CommandsManager.AddCommand("AnswerSelected", this.answerSelectedCommand);
                        this.networkManager.CommandsManager.AddCommand(this.mainPlayerConnecting);
                        this.networkManager.CommandsManager.AddCommand(this.presenterConnecting);

                        this.voteTimeoutTimer.Start();
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

            if (this.networkManager.CommandsManager.Exists("AnswerSelected"))
            {
                this.networkManager.CommandsManager.RemoveCommand("AnswerSelected");
            }

            if (this.networkManager.CommandsManager.Exists(this.mainPlayerConnecting))
            {
                this.networkManager.CommandsManager.RemoveCommand(this.mainPlayerConnecting);
            }

            if (this.networkManager.CommandsManager.Exists(this.presenterConnecting))
            {
                this.networkManager.CommandsManager.RemoveCommand(this.presenterConnecting);
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
        }
    }
}