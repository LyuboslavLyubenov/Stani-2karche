namespace Assets.Scripts.Interfaces.Network.Jokers
{
    using System;
    using System.Collections.Generic;

    using Assets.Scripts.EventArgs;

    public interface IAudienceAnswerPollRouter : IDisposable
    {
        event EventHandler OnActivated;

        event EventHandler<AudienceVoteEventArgs> OnVoteFinished;

        bool Activated { get; }

        void Activate(int timeToAnswerInSeconds, IEnumerable<int> clientsIdsThatMustVote, ISimpleQuestion question);

        void Deactivate();
    }
}
