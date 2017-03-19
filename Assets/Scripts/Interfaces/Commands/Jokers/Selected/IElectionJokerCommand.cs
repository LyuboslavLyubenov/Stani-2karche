namespace Assets.Scripts.Interfaces.Commands.Jokers.Selected
{
    using System;
    using EventArgs;

    public interface IElectionJokerCommand
    {
        event EventHandler OnAllPlayersSelected;

        event EventHandler<ClientConnectionIdEventArgs> OnPlayerSelected;

        event EventHandler OnSelectTimeout;
    }
}