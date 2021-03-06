namespace Commands.Jokers.Settings
{

    using System;
    using System.Collections.Generic;

    using Interfaces.Network.NetworkManager;

    using EventArgs = System.EventArgs;

    public class AskClientQuestionSettingsCommand : IOneTimeExecuteCommand
    {
        public delegate void OnReceivedSettings(int timeToAnswerInSeconds);

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

        private OnReceivedSettings onReceivedSettings;

        public AskClientQuestionSettingsCommand(OnReceivedSettings onReceivedSettings)
        {
            if (onReceivedSettings == null)
            {
                throw new ArgumentNullException("onReceivedSettings");
            }

            this.onReceivedSettings = onReceivedSettings;
        }

        public void Execute(Dictionary<string, string> commandsOptionsValues)
        {
            var timeToAnswerInSeconds = int.Parse(commandsOptionsValues["TimeToAnswerInSeconds"]);

            this.onReceivedSettings(timeToAnswerInSeconds);

            this.FinishedExecution = true;

            if (this.OnFinishedExecution != null)
            {
                this.OnFinishedExecution(this, EventArgs.Empty);
            }
        }
    }

}