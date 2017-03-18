namespace Interfaces.Network.Jokers.Routers
{

    using System;
    using System.Collections.Generic;

    using EventArgs;

    public interface IAnswerPollRouter : IJokerRouter, IDisposable
    {
        event EventHandler<VoteEventArgs> OnVoteFinished;

        bool Activated { get; }

        void Activate(int timeToAnswerInSeconds, IEnumerable<int> clientsIdsThatMustVote, ISimpleQuestion question);

        void Deactivate();
    }
}
