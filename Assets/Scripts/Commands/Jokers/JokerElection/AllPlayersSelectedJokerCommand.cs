using INetworkManagerCommand = Interfaces.Network.NetworkManager.INetworkManagerCommand;
using INetworkOperationExecutedCallback = Interfaces.Network.NetworkManager.INetworkOperationExecutedCallback;

namespace Commands.Jokers.JokerElection
{

    using System;
    using System.Collections.Generic;

    public class AllPlayersSelectedJokerCommand : INetworkManagerCommand, INetworkOperationExecutedCallback
    {
        public EventHandler OnExecuted
        {
            get; set;
        }

        public void Execute(Dictionary<string, string> commandsOptionsValues)
        {
            this.OnExecuted(this, EventArgs.Empty);
        }
    }
}