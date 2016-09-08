using System;

[Serializable]
public class ClientData_Serializable
{
    public int ConnectionId;
    public string Username;
}

[Serializable]
public class OnlineClientsData_Serializable
{
    public ClientData_Serializable[] OnlinePlayers;
}