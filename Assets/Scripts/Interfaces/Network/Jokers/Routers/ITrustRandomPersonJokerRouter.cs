using AnswerEventArgs = EventArgs.AnswerEventArgs;

namespace Interfaces.Network.Jokers.Routers
{

    using System;

    public interface ITrustRandomPersonJokerRouter : IJokerRouter
    {
        event EventHandler<AnswerEventArgs> OnReceivedAnswer;

        void Activate();
    }
}
