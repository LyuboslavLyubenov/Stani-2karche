namespace Assets.Scripts.Jokers.Routers
{

    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Assets.Scripts.Commands;
    using Assets.Scripts.Commands.Client;
    using Assets.Scripts.Commands.Jokers;
    using Assets.Scripts.Commands.Server;
    using Assets.Scripts.EventArgs;
    using Assets.Scripts.Extensions;
    using Assets.Scripts.Interfaces;
    using Assets.Scripts.Interfaces.Network.Jokers;
    using Assets.Scripts.Interfaces.Network.Jokers.Routers;
    using Assets.Scripts.Interfaces.Network.NetworkManager;
    using Assets.Scripts.Utils;

    using UnityEngine;

    public class AnswerPollRouter : IAnswerPollRouter
    {
        public const int MinTimeToAnswerInSeconds = 5;
        
        public event EventHandler OnActivated = delegate
            {
            };

        public event EventHandler<UnhandledExceptionEventArgs> OnError = delegate
            {
            };

        public event EventHandler<VoteEventArgs> OnVoteFinished = delegate
            {
            };
        
        private readonly IServerNetworkManager networkManager;

        private int timeToAnswerInSeconds;
        private int elapsedTime;

        private readonly List<int> clientsThatMustVote = new List<int>();
        private readonly List<int> votedClientsConnectionId = new List<int>();

        private readonly Dictionary<string, int> answersVotes = new Dictionary<string, int>();

        private Timer_ExecuteMethodEverySeconds updateTimeTimer;

        private ISimpleQuestion question;

        public bool Activated
        {
            get;
            private set;
        }

        public AnswerPollRouter(IServerNetworkManager networkManager)
        {
            if (networkManager == null)
            {
                throw new ArgumentNullException("networkManager");
            }
            
            this.networkManager = networkManager;

            this.updateTimeTimer = TimerUtils.ExecuteEvery(1f, this.UpdateTime);
            this.updateTimeTimer.RunOnUnityThread = true;
            this.updateTimeTimer.Stop();
        }

        private void OnReceivedVote(int connectionId, string answer)
        {
            if (!this.Activated ||
                !this.clientsThatMustVote.Contains(connectionId) ||
                this.votedClientsConnectionId.Contains(connectionId) ||
                !this.question.Answers.Contains(answer))
            {
                return;
            }
            
            this.answersVotes[answer]++;
            this.votedClientsConnectionId.Add(connectionId);

            if (this.AreFinishedVoting())
            {
                this.NoMoreTimeToAnswer();
                this.OnVoteFinished(this, new VoteEventArgs(this.answersVotes.ToDictionary(k => k.Key, v => v.Value)));
                return;
            }
        }

        private void UpdateTime()
        {
            if (!this.Activated)
            {
                return;
            }

            this.elapsedTime++;

            if (!this.AreFinishedVoting())
            {
                return;
            }

            this.TellClientsThatVotingIsOver();
            this.OnVoteFinished(this, new VoteEventArgs(this.answersVotes.ToDictionary(k => k.Key, v => v.Value)));
        }
        
        private void NoMoreTimeToAnswer()
        {
            var answerTimeoutCommandData = NetworkCommandData.From<AnswerTimeoutCommand>();

            for (int i = 0; i < this.clientsThatMustVote.Count; i++)
            {
                var clientId = this.clientsThatMustVote[i];
                this.networkManager.SendClientCommand(clientId, answerTimeoutCommandData);
            }
        }
        
        private bool AreFinishedVoting()
        {
            if (this.elapsedTime >= this.timeToAnswerInSeconds)
            {
                return true;
            }

            for (int i = 0; i < this.clientsThatMustVote.Count; i++)
            {
                var connectionId = this.clientsThatMustVote[i];

                if (!this.votedClientsConnectionId.Contains(connectionId))
                {
                    return false;
                }
            }

            return true;
        }

        private void SendSettings()
        {
            var audiencePollSettingsCommand = NetworkCommandData.From<AnswerPollSettingsCommand>();
            audiencePollSettingsCommand.AddOption("TimeToAnswerInSeconds", this.timeToAnswerInSeconds.ToString());

            for (int i = 0; i < this.clientsThatMustVote.Count; i++)
            {
                var clientId = this.clientsThatMustVote[i];
                this.networkManager.SendClientCommand(clientId, audiencePollSettingsCommand);
            }
        }

        private void SendQuestionToClients(ISimpleQuestion question)
        {
            var sendQuestionCommand = NetworkCommandData.From<LoadQuestionCommand>();
            var questionJSON = JsonUtility.ToJson(question.Serialize());
            sendQuestionCommand.AddOption("QuestionJSON", questionJSON);

            for (int i = 0; i < this.clientsThatMustVote.Count; i++)
            {
                var clientId = this.clientsThatMustVote[i];
                this.networkManager.SendClientCommand(clientId, sendQuestionCommand);
            }
        }

        private void TellClientsThatVotingIsOver()
        {
            var notificationCommand = new NetworkCommandData("ShowNotification");
            notificationCommand.AddOption("Color", "yellow");
            notificationCommand.AddOption("Message", "Voting is over!");

            for (int i = 0; i < this.clientsThatMustVote.Count; i++)
            {
                var clientId = this.clientsThatMustVote[i];
                this.networkManager.SendClientCommand(clientId, notificationCommand);
            }   
        }

        private void ResetAnswerVotes(ISimpleQuestion question)
        {
            this.answersVotes.Clear();

            for (int i = 0; i < question.Answers.Length; i++)
            {
                var answer = question.Answers[i];
                this.answersVotes.Add(answer, 0);
            }
        }
       
        public void Deactivate()
        {
            if (this.networkManager.CommandsManager.Exists("AnswerSelected"))
            {
                this.networkManager.CommandsManager.RemoveCommand("AnswerSelected");
            }
            
            this.TellClientsThatVotingIsOver();

            this.clientsThatMustVote.Clear();
            this.votedClientsConnectionId.Clear();
            this.answersVotes.Clear();
            this.question = null;

            this.timeToAnswerInSeconds = 0;
            this.elapsedTime = -1;

            this.Activated = false;
        }

        public void Activate(int timeToAnswerInSeconds, IEnumerable<int> clientsIdsThatMustVote, ISimpleQuestion question)
        {
            if (this.Activated)
            {
                throw new InvalidOperationException("Already active");
            }

            if (timeToAnswerInSeconds < MinTimeToAnswerInSeconds)
            {
                throw new ArgumentOutOfRangeException("timeToAnswerInSeconds");
            }

            if (clientsIdsThatMustVote == null || !clientsIdsThatMustVote.Any())
            {
                throw new ArgumentNullException("clientsIdsThatMustVote");
            }

            this.timeToAnswerInSeconds = timeToAnswerInSeconds;
            this.question = question;
            this.clientsThatMustVote.AddRange(clientsIdsThatMustVote);

            this.elapsedTime = 1;

            this.networkManager.CommandsManager.AddCommand("AnswerSelected", new SelectedAnswerCommand(this.OnReceivedVote));

            this.ResetAnswerVotes(question);
            this.SendSettings();
            this.SendQuestionToClients(question);
            
            this.updateTimeTimer.Reset();

            this.Activated = true;
            this.OnActivated(this, EventArgs.Empty);
        }

        /// <summary>
        /// when dispoing its calling and deactivate
        /// </summary>
        public void Dispose()
        {
            this.OnActivated = null;
            this.OnVoteFinished = null;

            try
            {
                this.updateTimeTimer.Stop();
            }
            finally
            {
                this.updateTimeTimer.Dispose();
                this.updateTimeTimer = null;
            }
        }
    }
}