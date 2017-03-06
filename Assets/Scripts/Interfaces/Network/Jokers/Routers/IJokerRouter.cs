namespace Assets.Scripts.Interfaces.Network.Jokers
{

    using System;

    public interface IJokerRouter
    {
        event EventHandler OnActivated;

        event EventHandler<UnhandledExceptionEventArgs> OnError;
    }

}