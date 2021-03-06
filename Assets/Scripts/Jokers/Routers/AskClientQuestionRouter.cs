namespace Jokers.Routers
{

    using System;

    using Commands;
    using Commands.Client;
    using Commands.Jokers.Settings;
    using Commands.Server;

    using EventArgs;

    using Interfaces;
    using Interfaces.Network.Jokers.Routers;
    using Interfaces.Network.NetworkManager;

    using UnityEngine;

    using Utils;

    using EventArgs = System.EventArgs;

    public class AskClientQuestionRouter : IAskClientQuestionRouter
    {
        public const int MinTimeToAnswerInSeconds = 5;

        public event EventHandler OnActivated = delegate
        {
        };

        public event EventHandler<AnswerEventArgs> OnReceivedAnswer = delegate
        {
        };

        public event EventHandler<UnhandledExceptionEventArgs> OnError = delegate
        {
        };

        public bool Active
        {
            get;
            private set;
        }

        private readonly IServerNetworkManager networkManager;

        private int clientConnectionId;

        private int timeToAnswerInSeconds;
        private int elapsedTime;

        private Timer_ExecuteMethodEverySeconds updateTimeTimer;

        private INetworkManagerCommand answerSelectedCommand;

        public AskClientQuestionRouter(IServerNetworkManager networkManager)
        {
            if (networkManager == null)
            {
                throw new ArgumentNullException("networkManager");
            }

            this.networkManager = networkManager;

            this.answerSelectedCommand = new SelectedAnswerOneTimeCommand(this.OnReceivedClientResponse);

            this.updateTimeTimer = TimerUtils.ExecuteEvery(1f, this.UpdateTimer);
            this.updateTimeTimer.RunOnUnityThread = true;
            this.updateTimeTimer.Stop();
        }

        private void UpdateTimer()
        {
            if (!this.Active)
            {
                return;
            }

            this.elapsedTime++;

            if (this.elapsedTime < this.timeToAnswerInSeconds)
            {
                return;
            }

            var answerTimeoutCommandData = new NetworkCommandData("AnswerTimeout");
            this.networkManager.SendClientCommand(this.clientConnectionId, answerTimeoutCommandData);

            this.OnError(this, new UnhandledExceptionEventArgs(new TimeoutException(), true));

            this.Deactivate();
        }

        private void OnReceivedClientResponse(int connectionId, string answer)
        {
            if (this.elapsedTime >= this.timeToAnswerInSeconds ||
                this.clientConnectionId != connectionId)
            {
                this.networkManager.CommandsManager.AddCommand("AnswerSelected", new SelectedAnswerOneTimeCommand(this.OnReceivedClientResponse));
                return;
            }

            this.OnReceivedAnswer(this, new AnswerEventArgs(answer, null));
        }

        private void SendQuestionToClient(ISimpleQuestion question)
        {
            var sendQuestionToFriend = NetworkCommandData.From<LoadQuestionCommand>();
            var questionJSON = JsonUtility.ToJson(question.Serialize());
            sendQuestionToFriend.AddOption("TimeToAnswer", this.timeToAnswerInSeconds.ToString());
            sendQuestionToFriend.AddOption("QuestionJSON", questionJSON);
            this.networkManager.SendClientCommand(this.clientConnectionId, sendQuestionToFriend);
        }

        private void SendSettings(int connectionId)
        {
            var settingsCommandData = NetworkCommandData.From<AskClientQuestionSettingsCommand>();
            settingsCommandData.AddOption("TimeToAnswerInSeconds", this.timeToAnswerInSeconds.ToString());
            this.networkManager.SendClientCommand(connectionId, settingsCommandData);
        }

        public void Activate(int clientConnectionId, int timeToAnswerInSeconds, ISimpleQuestion question)
        {
            if (this.Active)
            {
                throw new InvalidOperationException("Already active");
            }

            if (clientConnectionId <= 0)
            {
                throw new ArgumentOutOfRangeException("clientConnectionId", "Must be positive number");
            }

            if (timeToAnswerInSeconds < MinTimeToAnswerInSeconds)
            {
                throw new ArgumentOutOfRangeException("timeToAnswerInSeconds", "Time must be minimum " + MinTimeToAnswerInSeconds + " seconds");
            }

            if (question == null)
            {
                throw new ArgumentNullException("question");
            }
            
            this.clientConnectionId = clientConnectionId;
            this.timeToAnswerInSeconds = timeToAnswerInSeconds;

            this.SendSettings(this.clientConnectionId);
            this.SendQuestionToClient(question);

            this.networkManager.CommandsManager.AddCommand("AnswerSelected", answerSelectedCommand);

            this.Active = true;
            this.OnActivated(this, EventArgs.Empty);
        }

        public void Deactivate()
        {
            if (this.networkManager.CommandsManager.Exists(this.answerSelectedCommand))
            {
                this.networkManager.CommandsManager.RemoveCommand(this.answerSelectedCommand);
            }

            this.clientConnectionId = -1;
            this.elapsedTime = 0;
            this.Active = false;
        }

        public void Dispose()
        {
            this.OnActivated = null;
            this.OnReceivedAnswer = null;
            this.OnError = null;

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