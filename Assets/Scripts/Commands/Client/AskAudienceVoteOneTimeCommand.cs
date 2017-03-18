namespace Commands.Client
{

    using System;
    using System.Collections.Generic;

    using Interfaces.Network.NetworkManager;

    using EventArgs = System.EventArgs;

    public class AskAudienceVoteOneTimeCommand : IOneTimeExecuteCommand
    {
        public delegate void OnReceivedVote(int connectionId,string answer);

        private OnReceivedVote onReceivedVote;

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

        public AskAudienceVoteOneTimeCommand(OnReceivedVote onReceivedVote)
        {
            if (onReceivedVote == null)
            {
                throw new ArgumentNullException("onReceivedVote");
            }
            
            this.onReceivedVote = onReceivedVote;
        }

        public void Execute(Dictionary<string, string> commandsOptionsValues)
        {
            var connectionId = int.Parse(commandsOptionsValues["ClientConnectionId"]);
            var answer = commandsOptionsValues["Answer"];
            this.onReceivedVote(connectionId, answer);

            this.FinishedExecution = true;

            if (this.OnFinishedExecution != null)
            {
                this.OnFinishedExecution(this, EventArgs.Empty);
            }
        }
    }
}