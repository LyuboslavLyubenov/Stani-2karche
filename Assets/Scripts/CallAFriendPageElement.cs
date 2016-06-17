using System;

public class CallFriendPageElement
{
    public CallFriendPageElement(int connectionId, string name)
    {
        if (connectionId < 0)
        {
            throw new ArgumentOutOfRangeException("connectionId");
        }

        if (string.IsNullOrEmpty(name))
        {
            throw new ArgumentNullException("name");
        }

        this.ConnectionId = connectionId;
        this.Name = name;
    }

    public int ConnectionId
    {
        get;
        private set;
    }

    public string Name
    {
        get;
        private set;
    }
}