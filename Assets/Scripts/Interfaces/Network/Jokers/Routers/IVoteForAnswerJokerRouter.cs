namespace Assets.Scripts.Interfaces.Network.Jokers.Routers
{
    using System;
    using System.Collections.Generic;

    using Assets.Scripts.EventArgs;

    public interface IVoteForAnswerJokerRouter : IDisposable
    {
        event EventHandler<UnhandledExceptionEventArgs> OnError;

        void Activate(int timeToAnswerInSeconds, IEnumerable<int> clientsThatMustVote);
    }
}
