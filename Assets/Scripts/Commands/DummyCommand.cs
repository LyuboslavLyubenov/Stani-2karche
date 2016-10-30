using System;

//Mediator
using System.Collections.Generic;

public class DummyCommand : INetworkManagerCommand
{
    public EventHandler<DummyCommandReceivedDataEventArgs> OnExecuted = delegate
    {
    };

    public void Execute(Dictionary<string, string> commandsOptionsValues)
    {
        OnExecuted(this, new DummyCommandReceivedDataEventArgs(commandsOptionsValues));
    }
}