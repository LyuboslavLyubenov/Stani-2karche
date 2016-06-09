using System;

public class DataSentEventArgs : EventArgs
{

    public DataSentEventArgs(int connectionId, string message)
    {
        this.ConnectionId = connectionId;
        this.Message = message;
    }

    public int ConnectionId
    {
        get;
        private set;
    }

    public string Message
    {
        get;
        private set;
    }
}
