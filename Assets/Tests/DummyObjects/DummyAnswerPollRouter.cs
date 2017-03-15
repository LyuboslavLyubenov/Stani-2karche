namespace Assets.Tests.DummyObjects
{
    using System;
    using System.Collections.Generic;

    using Assets.Scripts.EventArgs;
    using Assets.Scripts.Interfaces;
    using Assets.Scripts.Interfaces.Network.Jokers.Routers;
    
    public class DummyAnswerPollRouter : IAnswerPollRouter
    {
        public event EventHandler OnActivated = delegate
            {
            };

        public event EventHandler<UnhandledExceptionEventArgs> OnError = delegate
            {
            };

        public event EventHandler<VoteEventArgs> OnVoteFinished = delegate
            {
            };

        public bool Activated
        {
            get; private set;
        }

        public void Activate(int timeToAnswerInSeconds, IEnumerable<int> clientsIdsThatMustVote, ISimpleQuestion question)
        {
            this.Activated = true;
            this.OnActivated(this, EventArgs.Empty);
        }

        public DummyAnswerPollRouter()
        {

        }

        public void Deactivate()
        {
            this.Activated = false;
        }

        public void Dispose()
        {
        }

        public void SimulateVoteFinished(Dictionary<string, int> answerVotes)
        {
            this.OnVoteFinished(this, new VoteEventArgs(answerVotes));
        }
    }
}
