using System.Collections.Generic;

/// <summary>
/// When executed is destroyed from commandsmanager class
/// </summary>
public interface IOneTimeExecuteCommand : INetworkManagerCommand
{
    bool FinishedExecution
    {
        get;
    }
}