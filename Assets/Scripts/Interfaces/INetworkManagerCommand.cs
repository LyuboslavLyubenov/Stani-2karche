using System.Collections.Generic;
using System;

public interface INetworkManagerCommand
{
    void Execute(Dictionary<string, string> commandsOptionsValues);
}