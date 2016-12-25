using System;
using System.Collections.Generic;

//Mediator

namespace Assets.Scripts.Commands
{

    using Assets.Scripts.EventArgs;
    using Assets.Scripts.Interfaces;

    public class DummyCommand : INetworkManagerCommand
    {
        public EventHandler<DummyCommandReceivedDataEventArgs> OnExecuted = delegate
            {
            };

        public void Execute(Dictionary<string, string> commandsOptionsValues)
        {
            this.OnExecuted(this, new DummyCommandReceivedDataEventArgs(commandsOptionsValues));
        }
    }

}