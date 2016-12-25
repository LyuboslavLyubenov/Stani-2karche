using System;
using System.Collections.Generic;

namespace Assets.Scripts.Commands.Jokers
{

    using Assets.Scripts.Interfaces;

    using EventArgs = System.EventArgs;

    public class AudiencePollSettingsCommand : IOneTimeExecuteCommand
    {
        public delegate void OnReceivedSettings(int timeToAnswerInSeconds);

        OnReceivedSettings onReceivedSettings;

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

        public AudiencePollSettingsCommand(OnReceivedSettings onReceivedSettings)
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

            if (this.OnFinishedExecution != null)
            {
                this.OnFinishedExecution(this, EventArgs.Empty);
            }
        }
    }

}