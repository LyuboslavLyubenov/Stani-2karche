namespace Assets.Scripts.Jokers
{

    using System;

    using Assets.Scripts.Commands;
    using Assets.Scripts.Commands.Client;
    using Assets.Scripts.Commands.Jokers;
    using Assets.Scripts.EventArgs;
    using Assets.Scripts.Interfaces.Network.Jokers;
    using Assets.Scripts.Network.NetworkManagers;
    using Assets.Scripts.Utils;

    using EventArgs = System.EventArgs;

    public class AskPlayerQuestionResultRetriever : IAskPlayerQuestionResultRetriever
    {
        public event EventHandler<AskPlayerResponseEventArgs> OnReceivedAnswer = delegate
            {
            };

        public event EventHandler<JokerSettingsEventArgs> OnReceivedSettings = delegate
            {
            };

        public event EventHandler OnReceiveAnswerTimeout = delegate
            {
            };

        public event EventHandler OnReceiveSettingsTimeout = delegate
            {
            };

        private ClientNetworkManager networkManager;

        private int receiveSettingsTimeout;

        private Timer_ExecuteMethodAfterTime timer;

        public bool Active
        {
            get;
            private set;
        }

        public AskPlayerQuestionResultRetriever(ClientNetworkManager networkManager, int receiveSettingsTimeout)
        {
            if (networkManager == null)
            {
                throw new ArgumentNullException("networkManager");
            }

            if (receiveSettingsTimeout <= 0)
            {
                throw new ArgumentOutOfRangeException("receiveSettingsTimeout");
            }

            this.networkManager = networkManager;
            this.receiveSettingsTimeout = receiveSettingsTimeout;
        }

        private void _OnReceivedSettings(int timeToAnswerInSeconds)
        {
            this.DisposeTimer();

            var responseCommand = new AskPlayerResponseCommand(this._OnReceivedAnswer);
            this.networkManager.CommandsManager.AddCommand(responseCommand);

            this.timer = TimerUtils.ExecuteAfter(timeToAnswerInSeconds, this.Timer_OnReceiveAnswerTimeout);
            this.timer.AutoDispose = true;
            this.timer.RunOnUnityThread = true;
            this.timer.Start();

            this.OnReceivedSettings(this, new JokerSettingsEventArgs(timeToAnswerInSeconds));
        }

        private void Timer_OnReceiveSettingsTimeout()
        {
            this.DisposeTimer();
            this.networkManager.CommandsManager.RemoveCommand<HelpFromFriendJokerSettingsCommand>();
            this.Active = false;
            this.OnReceiveSettingsTimeout(this, EventArgs.Empty);
        }

        private void _OnReceivedAnswer(string username, string answer)
        {
            this.DisposeTimer();
            this.Active = false;
            this.OnReceivedAnswer(this, new AskPlayerResponseEventArgs(username, answer));
        }

        private void Timer_OnReceiveAnswerTimeout()
        {
            this.DisposeTimer();

            this.networkManager.CommandsManager.RemoveCommand<HelpFromFriendJokerSettingsCommand>();
            this.Active = false;
            this.OnReceiveAnswerTimeout(this, EventArgs.Empty);
        }

        void DisposeTimer()
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

        public void Activate(int playerConnectionId)
        {
            var selected = NetworkCommandData.From<SelectedAskPlayerQuestionCommand>();
            selected.AddOption("PlayerConnectionId", playerConnectionId.ToString());

            this.networkManager.SendServerCommand(selected);

            var receivedSettingsCommand = new HelpFromFriendJokerSettingsCommand(this._OnReceivedSettings);
            this.networkManager.CommandsManager.AddCommand(receivedSettingsCommand);

            this.timer = TimerUtils.ExecuteAfter(this.receiveSettingsTimeout, this.Timer_OnReceiveSettingsTimeout);
            this.timer.RunOnUnityThread = true;
            this.timer.Start();

            this.Active = true;
        }

        public void Dispose()
        {
            this.OnReceivedAnswer = null;
            this.OnReceiveAnswerTimeout = null;
            this.OnReceivedSettings = null;
            this.OnReceiveSettingsTimeout = null;

            this.DisposeTimer();
        }
    }
}