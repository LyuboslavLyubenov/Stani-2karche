namespace Assets.Scripts.Jokers.Retrievers
{

    using System;
    using System.Timers;

    using Assets.Scripts.Commands.Client;
    using Assets.Scripts.Commands.Jokers;
    using Assets.Scripts.EventArgs;
    using Assets.Scripts.Interfaces.Network.Jokers;
    using Assets.Scripts.Interfaces.Network.NetworkManager;
    using Assets.Scripts.Utils;

    using EventArgs = System.EventArgs;

    public abstract class AnswerPollResultRetriever : IAnswerPollResultRetriever
    {
        public const int MinClientsForOnlineVote_Release = 4;
        public const int MinClientsForOnlineVote_Development = 1;

        public event EventHandler OnReceivedVoteTimeout = delegate
        {
        };

        public event EventHandler OnReceivedSettingsTimeout = delegate
        {
        };

        public event EventHandler<JokerSettingsEventArgs> OnReceivedSettings = delegate
        {
        };

        public event EventHandler<VoteEventArgs> OnVoted = delegate
        {
        };

        protected readonly IClientNetworkManager networkManager;
        private Timer timer;

        private readonly int receiveSettingsTimeoutInSeconds;

        public bool Activated
        {
            get;
            private set;
        }

        protected AnswerPollResultRetriever(IClientNetworkManager networkManager, int receiveSettingsTimeoutInSeconds)
        {
            if (networkManager == null)
            {
                throw new ArgumentNullException("networkManager");
            }

            if (receiveSettingsTimeoutInSeconds <= 0)
            {
                throw new ArgumentOutOfRangeException("receiveSettingsTimeoutInSeconds");
            }

            this.receiveSettingsTimeoutInSeconds = receiveSettingsTimeoutInSeconds;

            this.networkManager = networkManager;
            this.networkManager.OnDisconnectedEvent += this.OnDisconnected;
        }

        private void OnDisconnected(object sender, EventArgs args)
        {
            if (!this.Activated)
            {
                return;
            }

            this.DisposeTimer();

            if (this.networkManager.CommandsManager.Exists<AnswerPollSettingsCommand>())
            {
                this.networkManager.CommandsManager.RemoveCommand<AnswerPollSettingsCommand>();
            }
        }

        private void OnReceivedRouterSettings(int timeToAnswerInSeconds)
        {
            var receivedPollResultCommand =
                new AnswerPollResultCommand(
                    (votes) => this.OnVoted(this, new VoteEventArgs(votes)));

            this.networkManager.CommandsManager.AddCommand(receivedPollResultCommand);

            this.timer.Stop();
            this.timer.Dispose();

            this.timer = TimerUtils.ExecuteAfter(timeToAnswerInSeconds, this.Timer_OnReceiveVoteTimeout);
            ((IExtendedTimer)this.timer).RunOnUnityThread = true;
            this.timer.Start();

            this.OnReceivedSettings(this, new JokerSettingsEventArgs(timeToAnswerInSeconds));
        }

        private void DisposeTimer()
        {
            if (this.timer == null)
            {
                return;
            }

            try
            {
                this.timer.Stop();
            }
            finally
            {
                this.timer.Dispose();
                this.timer = null;
            }
        }

        private void Timer_OnReceiveVoteTimeout()
        {
            this.DisposeTimer();

            this.Activated = false;
            this.networkManager.CommandsManager.RemoveCommand<AnswerPollSettingsCommand>();

            this.OnReceivedVoteTimeout(this, EventArgs.Empty);
        }

        private void Timer_OnReceiveSettingsTimeout()
        {
            this.DisposeTimer();

            this.Activated = false;
            this.networkManager.CommandsManager.RemoveCommand<AnswerPollSettingsCommand>();

            this.OnReceivedSettingsTimeout(this, EventArgs.Empty);
        }
        
        /// <summary>
        /// Does not send request to the server. 
        /// Prepares client to receive AnswerPoll settings and AnswerPollVoteResult
        /// </summary>
        public virtual void Activate()
        {
            var receiveSettingsCommand = new AnswerPollSettingsCommand(this.OnReceivedRouterSettings);
            this.networkManager.CommandsManager.AddCommand(receiveSettingsCommand);

            this.timer = TimerUtils.ExecuteAfter(this.receiveSettingsTimeoutInSeconds, this.Timer_OnReceiveSettingsTimeout);
            this.timer.Start();

            ((IExtendedTimer)this.timer).RunOnUnityThread = true;

            this.Activated = true;
        }

        public virtual void Dispose()
        {
            this.DisposeTimer();
        }
    }
}