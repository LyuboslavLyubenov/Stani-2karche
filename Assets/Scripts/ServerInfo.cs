using System;

public class ServerInfo
{
    public string IPAddress
    {
        get;
        private set;
    }

    public int ConnectedClients
    {
        get;
        private set;
    }

    public int MaxConnectionsAllowed
    {
        get;
        private set;
    }

    public ServerInfo(string ipAddress, int connectedClients, int maxConnectionsAllowed)
    {
        if (!ipAddress.IsValidIPV4())
        {
            throw new ArgumentException("Invalid ipv4 address");
        }

        if (connectedClients < 0)
        {
            throw new ArgumentOutOfRangeException("connectedClients");
        }

        if (maxConnectionsAllowed < 0)
        {
            throw new ArgumentOutOfRangeException("maxConnectionsAllowed");
        }

        this.IPAddress = ipAddress;
        this.ConnectedClients = connectedClients;
        this.MaxConnectionsAllowed = maxConnectionsAllowed;
    }
}