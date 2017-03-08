namespace Assets.Scripts.Commands.Jokers
{
    using System;
    using System.Collections.Generic;

    using Assets.Scripts.Interfaces.Network.NetworkManager;

    using Interfaces;

    using EventArgs = System.EventArgs;

    public class DisableRandomAnswersJokerSettingsCommand : IOneTimeExecuteCommand
    {
        public delegate void OnReceivedJokerSettings(int answersToDisableCount);

        private OnReceivedJokerSettings onReceivedJokerSettings;

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

        public DisableRandomAnswersJokerSettingsCommand(OnReceivedJokerSettings onReceivedJokerSettings)
        {
            if (onReceivedJokerSettings == null)
            {
                throw new ArgumentNullException("onReceivedJokerSettings");
            }
            
            this.onReceivedJokerSettings = onReceivedJokerSettings;
        }

        public void Execute(Dictionary<string, string> commandsOptionsValues)
        {
            var answersToDisableCount = int.Parse(commandsOptionsValues["AnswersToDisableCount"]);
            this.onReceivedJokerSettings(answersToDisableCount);
            this.FinishedExecution = true;

            if (this.OnFinishedExecution != null)
            {
                this.OnFinishedExecution(this, EventArgs.Empty);
            }
        }
    }

}