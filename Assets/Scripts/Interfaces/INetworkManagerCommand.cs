using System.Collections.Generic;
using System;

public interface INetworkManagerCommand
{
    void Execute(Dictionary<string, string> commandsOptionsValues);
}

public interface ICommandExecutedCallback
{
    EventHandler OnExecuted
    {
        get;
        set;
    }
}