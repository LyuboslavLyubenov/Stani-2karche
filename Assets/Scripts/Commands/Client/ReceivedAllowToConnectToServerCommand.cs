using System;
using System.Collections.Generic;

public class ReceivedAllowToConnectToServerCommand : INetworkManagerCommand
{
    ValueWrapper<bool> isConnected;

    public ReceivedAllowToConnectToServerCommand(ValueWrapper<bool> isConnected)
    {
        if (isConnected == null)
        {
            throw new ArgumentNullException("isConnected");
        }

        this.isConnected = isConnected;
    }

    public void Execute(Dictionary<string, string> commandsParamsValues)
    {
        isConnected.Value = true;
    }
}
