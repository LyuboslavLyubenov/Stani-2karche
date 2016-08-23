using System;

[Serializable]
public class ServerInfo_Serializable
{
    public string ExternalIpAddress;
    public string LocalIPAddress;
    public int ConnectedClientsCount;
    public int MaxConnectionsAllowed;

    public bool IsFull
    {
        get
        {
            return ConnectedClientsCount >= MaxConnectionsAllowed;
        }
    }
}
