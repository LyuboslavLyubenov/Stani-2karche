using ClientConnectionIdEventArgs = EventArgs.ClientConnectionIdEventArgs;
using INetworkManagerCommand = Interfaces.Network.NetworkManager.INetworkManagerCommand;

namespace Assets.Scripts.Interfaces.Commands.Jokers.Selected
{
    using System;
    using EventArgs;

    public interface IElectionJokerCommand : INetworkManagerCommand
    {
        event EventHandler OnAllPlayersSelected;

        event EventHandler<ClientConnectionIdEventArgs> OnPlayerSelected;

        event EventHandler OnSelectTimeout;
    }
}