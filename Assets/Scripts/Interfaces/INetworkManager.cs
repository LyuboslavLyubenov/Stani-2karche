using System;

public interface INetworkManager
{
    bool IsRunning
    {
        get;
    }

    EventHandler OnConnectedEvent
    {
        get;
        set;
    }

    EventHandler<DataSentEventArgs> OnReceivedDataEvent
    {
        get;
        set;
    }

    EventHandler OnDisconnectedEvent
    {
        get;
        set;
    }
}
