using ClientConnectionIdEventArgs = EventArgs.ClientConnectionIdEventArgs;
using INetworkManagerCommand = Interfaces.Network.NetworkManager.INetworkManagerCommand;

namespace Interfaces.Commands.Jokers.Selected
{

    using System;

    using EventArgs.Jokers;

    public interface IElectionJokerCommand : INetworkManagerCommand
    {
        event EventHandler OnElectionStarted;

        event EventHandler<ElectionJokerResultEventArgs> OnElectionResult;

        event EventHandler<ClientConnectionIdEventArgs> OnPlayerSelectedFor;

        event EventHandler<ClientConnectionIdEventArgs> OnPlayerSelectedAgainst; 
    }
}