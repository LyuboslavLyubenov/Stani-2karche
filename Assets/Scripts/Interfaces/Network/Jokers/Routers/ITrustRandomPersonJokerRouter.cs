using IJokerRouter = Interfaces.Network.Jokers.Routers.IJokerRouter;

namespace Assets.Scripts.Interfaces.Network.Jokers.Routers
{

    using System;

    using EventArgs;

    public interface ITrustRandomPersonJokerRouter : IJokerRouter
    {
        event EventHandler<AnswerEventArgs> OnReceivedAnswer;

        void Activate();
    }
}
