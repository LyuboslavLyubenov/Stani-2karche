namespace Commands.Server
{

    using System;
    using System.Collections.Generic;

    using Interfaces.Network.NetworkManager;

    public class SelectedAnswerCommand : INetworkManagerCommand
    {
        public delegate void OnReceivedAnswerDelegate(int clientId,string answer);

        private OnReceivedAnswerDelegate onReceivedAnswer;

        public SelectedAnswerCommand(OnReceivedAnswerDelegate onReceivedAnswer)
        {
            if (onReceivedAnswer == null)
            {
                throw new ArgumentNullException("onReceivedAnswer");
            }

            this.onReceivedAnswer = onReceivedAnswer;
        }

        public virtual void Execute(Dictionary<string, string> commandsOptionsValues)
        {
            var connectionId = int.Parse(commandsOptionsValues["ConnectionId"]);
            var answerSelected = commandsOptionsValues["Answer"];

            this.onReceivedAnswer(connectionId, answerSelected);
        }
    }

    public class SelectedAnswerOneTimeCommand : SelectedAnswerCommand, IOneTimeExecuteCommand
    {
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

        public SelectedAnswerOneTimeCommand(OnReceivedAnswerDelegate onReceivedAnswer)
            : base(onReceivedAnswer)
        {
            this.FinishedExecution = false;
        }

        public override void Execute(Dictionary<string, string> commandsOptionsValues)
        {
            base.Execute(commandsOptionsValues);
            this.FinishedExecution = true;

            if (this.OnFinishedExecution != null)
            {
                this.OnFinishedExecution(this, EventArgs.Empty);
            }
        }
    }
}