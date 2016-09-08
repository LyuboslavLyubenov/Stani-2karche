using System.Collections.Generic;

public interface INetworkManagerCommand
{
    void Execute(Dictionary<string, string> commandsOptionsValues);
}
