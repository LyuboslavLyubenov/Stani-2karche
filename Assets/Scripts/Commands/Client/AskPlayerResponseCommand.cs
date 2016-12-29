﻿namespace Assets.Scripts.Commands.Client
{
    using System;

    using Interfaces;

    using EventArgs = System.EventArgs;

    public class AskPlayerResponseCommand : IOneTimeExecuteCommand
    {
        public delegate void OnReceivedAnswer(string username,string answer);

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

        private OnReceivedAnswer onReceivedAnswer;

        public AskPlayerResponseCommand(OnReceivedAnswer onReceivedAnswer)
        {
            if (onReceivedAnswer == null)
            {
                throw new ArgumentNullException("onReceivedAnswer");
            }
            
            this.onReceivedAnswer = onReceivedAnswer;

            this.FinishedExecution = false;
        }

        public void Execute(System.Collections.Generic.Dictionary<string, string> commandsOptionsValues)
        {
            var username = commandsOptionsValues["Username"];
            var answer = commandsOptionsValues["Answer"];

            this.onReceivedAnswer(username, answer);

            this.FinishedExecution = true;

            if (this.OnFinishedExecution != null)
            {
                this.OnFinishedExecution(this, EventArgs.Empty);
            }
        }
    }
}