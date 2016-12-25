﻿using System;
using System.Collections.Generic;

namespace Assets.Scripts.Commands.Client
{

    using Assets.Scripts.Interfaces;

    using EventArgs = System.EventArgs;

    public class ReceivedAskAudienceVoteOneTimeCommand : IOneTimeExecuteCommand
    {
        public delegate void OnReceivedVote(int connectionId,string answer);

        OnReceivedVote onReceivedVote;

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

        public ReceivedAskAudienceVoteOneTimeCommand(OnReceivedVote onReceivedVote)
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
