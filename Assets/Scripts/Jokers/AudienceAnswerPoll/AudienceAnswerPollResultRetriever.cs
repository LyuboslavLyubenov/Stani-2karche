namespace Assets.Scripts.Jokers.AudienceAnswerPoll
{
    using System;
    using System.Timers;

    using UnityEngine;

    using Commands;
    using Commands.Client;
    using Commands.Jokers;
    using EventArgs;

    using Network.NetworkManagers;
    using Utils;

    using EventArgs = System.EventArgs;

    public class AudienceAnswerPollResultRetriever : IDisposable
    {
        public const int MinClientsForOnlineVote_Release = 4;
        public const int MinClientsForOnlineVote_Development = 1;

        public event EventHandler<AudienceVoteEventArgs> OnAudienceVoted = delegate
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

        private readonly ClientNetworkManager networkManager;
        private Timer timer;

        private int receiveSettingsTimeoutInSeconds;

        public EventHandler OnActivated
        {
            get;
            set;
        }

        public bool Activated
        {
            get;
            private set;
        }

        public AudienceAnswerPollResultRetriever(ClientNetworkManager networkManager, int receiveSettingsTimeoutInSeconds)
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

            DisposeTimer();
            this.networkManager.CommandsManager.RemoveCommand<AudiencePollSettingsCommand>();
        }

        private void OnReceivedJokerSettings(int timeToAnswerInSeconds)
        {
            var receivedAskAudienceVoteResultCommand =
                new AudiencePollResultCommand(
                    (votes) => this.OnAudienceVoted(this, new AudienceVoteEventArgs(votes)));

            this.networkManager.CommandsManager.AddCommand(receivedAskAudienceVoteResultCommand);

            this.timer.Dispose();
            this.timer = TimerUtils.ExecuteAfter(timeToAnswerInSeconds, Timer_OnReceiveAudienceVoteTimeout);
            this.timer.Start();

            ((IExtendedTimer)this.timer).RunOnUnityThread = true;

            this.OnReceivedSettings(this, new JokerSettingsEventArgs(timeToAnswerInSeconds));
        }

        private void DisposeTimer()
        {
            if (timer == null)
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

        private void Timer_OnReceiveAudienceVoteTimeout()
        {
            DisposeTimer();

            this.Activated = false;
            this.networkManager.CommandsManager.RemoveCommand<AudiencePollSettingsCommand>();

            this.OnReceiveAudienceVoteTimeout(this, EventArgs.Empty);
        }

        private void Timer_OnReceiveSettingsTimeout()
        {
            DisposeTimer();

            this.Activated = false;
            this.networkManager.CommandsManager.RemoveCommand<AudiencePollSettingsCommand>();

            this.OnReceiveSettingsTimeout(this, EventArgs.Empty);
        }

        public void Activate()
        {
            var selected = NetworkCommandData.From<SelectedAudiencePollCommand>();
            this.networkManager.SendServerCommand(selected);

            var receiveSettingsCommand = new AudiencePollSettingsCommand(this.OnReceivedJokerSettings);
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
            this.OnAudienceVoted = null;
            this.OnReceiveAudienceVoteTimeout = null;
            this.OnReceiveSettingsTimeout = null;
            this.OnReceivedSettings = null;

            this.DisposeTimer();
        }
    }
}