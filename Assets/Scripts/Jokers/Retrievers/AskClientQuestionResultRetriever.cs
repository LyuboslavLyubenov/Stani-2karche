namespace Jokers.Retrievers
{

    using System;

    using Assets.Scripts.Commands.Client;
    using Assets.Scripts.Commands.Jokers;
    using Assets.Scripts.EventArgs;
    using Assets.Scripts.Interfaces.Network.NetworkManager;
    using Assets.Scripts.Utils;

    using Interfaces.Network.Jokers;

    public abstract class AskClientQuestionResultRetriever : IAskClientQuestionResultRetriever
    {
        public event EventHandler<AskClientQuestionResponseEventArgs> OnReceivedAnswer = delegate
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

        protected IClientNetworkManager networkManager;

        private int receiveSettingsTimeout;

        private Timer_ExecuteMethodAfterTime timer;

        public bool Active
        {
            get;
            private set;
        }

        protected AskClientQuestionResultRetriever(IClientNetworkManager networkManager, int receiveSettingsTimeout)
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

            var responseCommand = new AskClientQuestionResponseCommand(this._OnReceivedAnswer);
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
            this.networkManager.CommandsManager.RemoveCommand<AskClientQuestionSettingsCommand>();
            this.Active = false;
            this.OnReceiveSettingsTimeout(this, EventArgs.Empty);
        }

        private void _OnReceivedAnswer(string username, string answer)
        {
            this.DisposeTimer();
            this.Active = false;
            this.OnReceivedAnswer(this, new AskClientQuestionResponseEventArgs(username, answer));
        }

        private void Timer_OnReceiveAnswerTimeout()
        {
            this.DisposeTimer();
            
            this.networkManager.CommandsManager.RemoveCommand<AskClientQuestionSettingsCommand>();
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

        /// <summary>
        /// clientConnectionId not used in this method. 
        /// Use it as template. 
        /// Should send command to server containing this id before activating
        /// </summary>
        public virtual void Activate(int clientConnectionId)
        {
            if (this.Active)
            {
                throw new InvalidOperationException("Already started");
            }

            if (clientConnectionId <= 0)
            {
                throw new ArgumentOutOfRangeException("clientConnectionId");
            }

            var receivedSettingsCommand = new AskClientQuestionSettingsCommand(this._OnReceivedSettings);
            this.networkManager.CommandsManager.AddCommand(receivedSettingsCommand);

            this.timer = TimerUtils.ExecuteAfter(this.receiveSettingsTimeout, this.Timer_OnReceiveSettingsTimeout);
            this.timer.RunOnUnityThread = true;
            this.timer.Start();

            this.Active = true;
        }

        public virtual void Deactivate()
        {
            this.DisposeTimer();

            if (this.networkManager.CommandsManager.Exists<AskClientQuestionSettingsCommand>())
            {
                this.networkManager.CommandsManager.RemoveCommand<AskClientQuestionSettingsCommand>();
            }

            if (this.networkManager.CommandsManager.Exists<AskClientQuestionResponseCommand>())
            {
                this.networkManager.CommandsManager.RemoveCommand<AskClientQuestionResponseCommand>();
            }

            this.Active = false;
        }

        public virtual void Dispose()
        {
            this.Deactivate();

            this.OnReceivedAnswer = null;
            this.OnReceiveAnswerTimeout = null;
            this.OnReceivedSettings = null;
            this.OnReceiveSettingsTimeout = null;
        }
    }

}