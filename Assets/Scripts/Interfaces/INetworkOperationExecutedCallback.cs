using System;

public interface INetworkOperationExecutedCallback
{
    EventHandler OnExecuted
    {
        get;
        set;
    }
}