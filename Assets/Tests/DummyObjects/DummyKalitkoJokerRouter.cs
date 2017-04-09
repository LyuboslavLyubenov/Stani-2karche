namespace Tests.DummyObjects
{

    using System;

    using Interfaces.Network.Jokers.Routers;

    public class DummyKalitkoJokerRouter : IKalitkoJokerRouter
    {
        public event EventHandler OnActivated = delegate
            {
            };

        public event EventHandler<UnhandledExceptionEventArgs> OnError = delegate
            {
            };

        public void Activate()
        {
            this.OnActivated(this, EventArgs.Empty);
        }
    }
}
