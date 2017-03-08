namespace Assets.Scripts.Interfaces.Network.Jokers.Routers
{

    using System;

    public interface IJokerRouter
    {
        event EventHandler OnActivated;

        event EventHandler<UnhandledExceptionEventArgs> OnError;
    }

}