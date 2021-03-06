namespace Commands.Jokers.Settings
{

    using System;
    using System.Collections.Generic;

    using Interfaces.Network.NetworkManager;

    using Utils.Unity;

    using EventArgs = System.EventArgs;

    public class AskFriendJokerSettingsCommand : IOneTimeExecuteCommand
    {
        private DisableAfterDelay disableAfterDelay;

        private Action onAppliedSettings;

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

        public AskFriendJokerSettingsCommand(DisableAfterDelay disableAfterDelay, Action onAppliedSettings)
        {
            if (disableAfterDelay == null)
            {
                throw new ArgumentNullException("disableAfterDelay");
            }

            if (onAppliedSettings == null)
            {
                throw new ArgumentNullException("onAppliedSettings");
            }
            
            this.disableAfterDelay = disableAfterDelay;
            this.onAppliedSettings = onAppliedSettings;
        }

        public void Execute(Dictionary<string, string> commandsOptionsValues)
        {
            var timeToAnswerInSeconds = int.Parse(commandsOptionsValues["TimeToAnswerInSeconds"]);
            this.disableAfterDelay.InvervalInSeconds = timeToAnswerInSeconds;
            this.onAppliedSettings();
            this.FinishedExecution = true;

            if (this.OnFinishedExecution != null)
            {
                this.OnFinishedExecution(this, EventArgs.Empty);
            }
        }
    }

}