//Mediator

namespace Commands
{

    using System;
    using System.Collections.Generic;

    using EventArgs;

    using Interfaces.Network.NetworkManager;

    public class DummyCommand : INetworkManagerCommand
    {
        public event EventHandler<DummyCommandReceivedDataEventArgs> OnExecuted = delegate
            {
            };

        public void Execute(Dictionary<string, string> commandsOptionsValues)
        {
            this.OnExecuted(this, new DummyCommandReceivedDataEventArgs(commandsOptionsValues));
        }
    }

}