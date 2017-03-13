namespace Assets.Scripts.Jokers
{

    using System;
    using System.Timers;

    using Assets.Scripts.Commands;
    using Assets.Scripts.Commands.Client;
    using Assets.Scripts.Commands.Jokers;
    using Assets.Scripts.EventArgs;
    using Assets.Scripts.Interfaces.Network.Jokers;
    using Assets.Scripts.Interfaces.Network.NetworkManager;
    using Assets.Scripts.Utils;

    using EventArgs = System.EventArgs;

    public class AnswerPollResultRetrieverFromClient : IAnswerPollResultRetriever
    {
        public const int MinClientsForOnlineVote_Release = 4;
        public const int MinClientsForOnlineVote_Development = 1;

        public event EventHandler<VoteEventArgs> OnVoted = delegate
            {
            };

        public event EventHandler<JokerSettingsEventArgs> OnReceivedSettings = delegate
            {
            };

        public event EventHandler OnReceiveSettingsTimeout = delegate
            {
            };

        public event EventHandler OnReceiveAudienceVoteTimeout = delegate
            {
            };

        public event EventHandler OnActivated = delegate
        {
        };

        private readonly IClientNetworkManager networkManager;
        private Timer timer;

        private int receiveSettingsTimeoutInSeconds;
        
        public bool Activated
        {
            get;
            private set;
        }

        public AnswerPollResultRetrieverFromClient(IClientNetworkManager networkManager, int receiveSettingsTimeoutInSeconds)
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
            this.networkManager.CommandsManager.RemoveCommand<AnswerPollSettingsCommand>();
        }

        private void OnReceivedJokerSettings(int timeToAnswerInSeconds)
        {
            var receivedPollResultCommand =
                new AnswerPollResultCommand(
                    (votes) => this.OnVoted(this, new VoteEventArgs(votes)));

            this.networkManager.CommandsManager.AddCommand(receivedPollResultCommand);

            this.timer.Stop();
            this.timer.Dispose();
            this.timer = TimerUtils.ExecuteAfter(timeToAnswerInSeconds, this.Timer_OnReceiveVoteTimeout);
            this.timer.Start();

            ((IExtendedTimer)this.timer).RunOnUnityThread = true;

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

            this.OnReceiveAudienceVoteTimeout(this, EventArgs.Empty);
        }

        private void Timer_OnReceiveSettingsTimeout()
        {
            this.DisposeTimer();

            this.Activated = false;
            this.networkManager.CommandsManager.RemoveCommand<AnswerPollSettingsCommand>();

            this.OnReceiveSettingsTimeout(this, EventArgs.Empty);
        }

        public void Activate()
        {
            var selected = NetworkCommandData.From<SelectedAnswerPollCommand>();
            this.networkManager.SendServerCommand(selected);

            var receiveSettingsCommand = new AnswerPollSettingsCommand(this.OnReceivedJokerSettings);
            this.networkManager.CommandsManager.AddCommand(receiveSettingsCommand);

            this.timer = TimerUtils.ExecuteAfter(this.receiveSettingsTimeoutInSeconds, this.Timer_OnReceiveSettingsTimeout);
            this.timer.Start();

            ((IExtendedTimer)this.timer).RunOnUnityThread = true;

            this.Activated = true;

            if (this.OnActivated != null)
            {
                this.OnActivated(this, EventArgs.Empty);
            }
        }

        public void Dispose()
        {
            this.OnVoted = null;
            this.OnReceiveAudienceVoteTimeout = null;
            this.OnReceiveSettingsTimeout = null;
            this.OnReceivedSettings = null;

            this.DisposeTimer();
        }
    }
}