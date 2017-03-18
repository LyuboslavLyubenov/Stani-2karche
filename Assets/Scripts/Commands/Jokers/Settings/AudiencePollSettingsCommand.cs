namespace Commands.Jokers.Settings
{

    using System;
    using System.Collections.Generic;

    using Interfaces.Network.NetworkManager;

    using EventArgs = System.EventArgs;

    public class AnswerPollSettingsCommand : IOneTimeExecuteCommand
    {
        public delegate void OnReceivedSettings(int timeToAnswerInSeconds);

        private OnReceivedSettings onReceivedSettings;

        public bool FinishedExecution
        {
            get;
            private set;
        }

        public EventHandler OnFinishedExecution
        {
            get;
            set;
        }

        public AnswerPollSettingsCommand(OnReceivedSettings onReceivedSettings)
        {
            if (onReceivedSettings == null)
            {
                throw new ArgumentNullException("onReceivedSettings");
            }
            
            this.onReceivedSettings = onReceivedSettings;
        }

        public void Execute(Dictionary<string, string> commandsOptionsValues)
        {
            var timeToAnswer = int.Parse(commandsOptionsValues["TimeToAnswerInSeconds"]);
            this.onReceivedSettings(timeToAnswer);

            this.FinishedExecution = true;

            if (this.OnFinishedExecution != null)
            {
                this.OnFinishedExecution(this, EventArgs.Empty);
            }
        }
    }

}