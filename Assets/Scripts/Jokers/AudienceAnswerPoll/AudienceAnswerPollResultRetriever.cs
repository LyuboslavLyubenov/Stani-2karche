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

    public class AudienceAnswerPollResultRetriever : MonoBehaviour
    {
        public const int MinClientsForOnlineVote_Release = 4;
        public const int MinClientsForOnlineVote_Development = 1;

        private const int SettingsReceiveTimeoutInSeconds = 10;

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

        private ClientNetworkManager networkManager;
        private Timer timer;

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

        public AudienceAnswerPollResultRetriever(ClientNetworkManager networkManager)
        {
            if (networkManager == null)
            {
                throw new ArgumentNullException("networkManager");
            }

            this.networkManager = networkManager;
            this.networkManager.OnDisconnectedEvent += this.OnDisconnected;
        }

        private void OnDisconnected(object sender, EventArgs args)
        {
            if (!this.Activated)
            {
                return;
            }

            try
            {
                this.timer.Close();
                this.networkManager.CommandsManager.RemoveCommand<AudiencePollSettingsCommand>();
            }
            catch
            {
            }
        }

        private void OnReceivedJokerSettings(int timeToAnswerInSeconds)
        {
            var receivedAskAudienceVoteResultCommand =
                new AudiencePollResultCommand(
                    (votes) => this.OnAudienceVoted(this, new AudienceVoteEventArgs(votes)));

            this.networkManager.CommandsManager.AddCommand(receivedAskAudienceVoteResultCommand);

            this.timer.Dispose();
            this.timer = TimerUtils.ExecuteAfter(SettingsReceiveTimeoutInSeconds * 1000, Timer_OnReceiveAudienceVoteTimeout);
            this.timer.Start();

            ((IExtendedTimer)this.timer).RunOnUnityThread = true;

            this.OnReceivedSettings(this, new JokerSettingsEventArgs(timeToAnswerInSeconds));
        }

        void DisposeTimer()
        {
            this.timer.Dispose();
            this.timer = null;
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

            this.timer = TimerUtils.ExecuteAfter(SettingsReceiveTimeoutInSeconds, this.Timer_OnReceiveSettingsTimeout);
            this.timer.Start();

            ((IExtendedTimer)this.timer).RunOnUnityThread = true;
            
            this.Activated = true;

            if (this.OnActivated != null)
            {
                this.OnActivated(this, EventArgs.Empty);
            }
        }
    }

}