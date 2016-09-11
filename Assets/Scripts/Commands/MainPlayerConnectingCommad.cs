using System;
using System.Collections.Generic;

public class MainPlayerConnectingCommand : INetworkManagerCommand
{
    public delegate void MainPlayerConnectingDelegate(int connectionId);

    MainPlayerConnectingDelegate onMainPlayerConnecting;

    public MainPlayerConnectingCommand(MainPlayerConnectingDelegate onMainPlayerConnecting)
    {
        if (onMainPlayerConnecting == null)
        {
            throw new ArgumentNullException("onMainPlayerConnecting");
        }

        this.onMainPlayerConnecting = onMainPlayerConnecting;
    }

    public void Execute(Dictionary<string, string> commandsOptionsValues)
    {
        var connectionId = int.Parse(commandsOptionsValues["ConnectionId"]);
        onMainPlayerConnecting(connectionId);
    }

}